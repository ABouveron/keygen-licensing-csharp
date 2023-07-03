// ReSharper disable CommentTypo

namespace example_csharp_licensing_Docker;

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Management;
using System.Diagnostics.CodeAnalysis;
using NSec.Cryptography;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using HashLib;

public abstract partial class Program
{
  private static string GetSerialNumber() 
  {
    try
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        return new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS")
          .Get()
          .Cast<ManagementObject>()
          .First()
          .Properties["SerialNumber"]
          .Value
          .ToString();
      }
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        const string path = "/sys/class/dmi/id/product_serial";
        string serialNumber = File.ReadAllText(path);
        return serialNumber.Remove((serialNumber.Length -1), 1);
      }
      else {
        return null;
      }
    }
    catch(Exception e) {
      Console.WriteLine($"Impossible de récupérer le numéro de série : {e.Message}");
      Environment.Exit(1);
      return null;
    }
  }

  private const string LicenseKey = "key/TEg3TS05VldLLUpKSFUtN0NSVC1NUEtSLUg5VUwtOU1GNy03VjlK.hphP_9YaFq0uZykkfH0l9xEmogJ4yUbo3Wym7oIxYgl0uNBwocsS3GZse6U2Ti2a8B09iB5-gi_ilr3V05z4Dw==";
  private const string PublicKey = "7757a98a8188c31ae7a21d76a865800bf77bcf3476f7abbbdf5bb6a4afbe9a23";

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class LicenseFile
  {
    public string enc { get; set; }
    public string sig { get; set; }
    public string alg { get; set; }
  }

  public static void Main()
  {
    const string pathLicenseFile = "machine.lic";
    var licenseFileRaw = File.ReadAllText(pathLicenseFile);
  
    string serialNumber = GetSerialNumber();
    if (serialNumber is null) {
      Console.WriteLine("Impossible de récupérer le numéro de série.");
      Environment.Exit(1);
    }
    else {
      Console.WriteLine("Serial number : " + serialNumber);
    }

    var hashAlgorithm = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);

    byte[] serialNumberBytes = Encoding.UTF8.GetBytes(serialNumber);
    hashAlgorithm.BlockUpdate(serialNumberBytes, 0, serialNumberBytes.Length);
    byte[] result = new byte[hashAlgorithm.GetDigestSize()];
    hashAlgorithm.DoFinal(result, 0);
    string fingerprint = BitConverter.ToString(result);
    fingerprint = fingerprint.Replace("-", "").ToLower();

    // Parse signed license file (removing cert header, newlines and footer)
    var encodedPayload = Regex.Replace(licenseFileRaw,
      "(^-----BEGIN MACHINE FILE-----\\n|\\n|-----END MACHINE FILE-----\\n$)", "");
    var payloadBytes = Convert.FromBase64String(encodedPayload);
    var payload = Encoding.UTF8.GetString(payloadBytes);
    string encryptedData;
    string encodedSignature;
    string algorithm;

    // Deserialize license file certificate
    try
    {
      var lic = JsonSerializer.Deserialize<LicenseFile>(payload);

      encryptedData = lic.enc;
      encodedSignature = lic.sig;
      algorithm = lic.alg;
    }
    catch (JsonException e)
    {
      Console.WriteLine($"Failed to parse machine file: {e.Message}");

      return;
    }

    // Verify license file algorithm
    if (algorithm != "aes-256-gcm+ed25519")
    {
      Console.WriteLine("Unsupported algorithm!");

      return;
    }

    // Verify signature
    var ed25519 = SignatureAlgorithm.Ed25519;
    var signatureBytes = Convert.FromBase64String(encodedSignature);
    var signingDataBytes = Encoding.UTF8.GetBytes($"machine/{encryptedData}");
    var publicKeyBytes = Convert.FromHexString(PublicKey);
    var key = NSec.Cryptography.PublicKey.Import(ed25519, publicKeyBytes, KeyBlobFormat.RawPublicKey);

    if (ed25519.Verify(key, signingDataBytes, signatureBytes))
    {
      Console.WriteLine("Machine file is valid! Decrypting...");

      // Decrypt license file dataset
      string plaintext;
      try
      {
        var encodedCipherText = encryptedData.Split(".", 3)[0];
        var encodedIv = encryptedData.Split(".", 3)[1];
        var encodedTag = encryptedData.Split(".", 3)[2];
        var cipherText = Convert.FromBase64String(encodedCipherText);
        var iv = Convert.FromBase64String(encodedIv);
        var tag = Convert.FromBase64String(encodedTag);
        byte[] secret;

        // Hash license key to get decryption secret 
        try
        {
          var licenseKeyBytes = Encoding.UTF8.GetBytes(LicenseKey);
          var fingerprintBytes = Encoding.UTF8.GetBytes(fingerprint);
          var sha256 = new Sha256();

          secret = sha256.Hash(licenseKeyBytes.Concat(fingerprintBytes).ToArray());
        }
        catch (Exception e)
        {
          Console.WriteLine($"Failed to hash license key: {e.Message}");

          return;
        }

        // Init AES-GCM
        var cipherParams = new AeadParameters(new KeyParameter(secret), 128, iv);
        var aesEngine = new AesEngine();
        var cipher = new GcmBlockCipher(aesEngine);

        cipher.Init(false, cipherParams);

        // Concat auth tag to ciphertext
        var input = cipherText.Concat(tag).ToArray();
        var output = new byte[cipher.GetOutputSize(input.Length)];

        // Decrypt
        var len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
        cipher.DoFinal(output, len);

        // Convert decrypted bytes to string
        plaintext = Encoding.UTF8.GetString(output);
      }
      catch (Exception e)
      {
        Console.WriteLine($"Failed to decrypt machine file: {e.Message}");

        return;
      }

      Console.WriteLine("Machine file was successfully decrypted!");
      //Console.WriteLine($"Decrypted: {plaintext}");
    }
    else
    {
      Console.WriteLine("Invalid machine file!");
    }
  }

    [GeneratedRegex("(^-----BEGIN MACHINE FILE-----\\n|\\n|-----END MACHINE FILE-----\\n$)")]
    private static partial Regex MyRegex();
}