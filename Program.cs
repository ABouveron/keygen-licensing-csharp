// ReSharper disable CommentTypo

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using NSec.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace example_csharp_licensing_Docker;

public abstract partial class Program
{
    // Definition of all constant and variables needed to verify and decrypt the license file
    private const string LicenseKey =
        "key/TEg3TS05VldLLUpKSFUtN0NSVC1NUEtSLUg5VUwtOU1GNy03VjlK.hphP_9YaFq0uZykkfH0l9xEmogJ4yUbo3Wym7oIxYgl0uNBwocsS3GZse6U2Ti2a8B09iB5-gi_ilr3V05z4Dw==";
    private const string PublicKey = "7757a98a8188c31ae7a21d76a865800bf77bcf3476f7abbbdf5bb6a4afbe9a23";
    
    // Method to get EUID on Linux
    [DllImport("libc")]
    [SuppressMessage("Interoperability", "SYSLIB1054:Utilisez «\u00a0LibraryImportAttribute\u00a0» à la place de «\u00a0DllImportAttribute\u00a0» pour générer du code de marshaling P/Invoke au moment de la compilation")]
    private static extern uint geteuid();

    // Method to get serial number of the machine
    private static string GetSerialNumber()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS")
                    .Get()
                    .Cast<ManagementObject>()
                    .First()
                    .Properties["SerialNumber"]
                    .Value
                    .ToString();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return null;
            if (geteuid() != 0)
            {
                Console.WriteLine("You must be root to get the serial number. Execute again with \"sudo dotnet run\".");
                Environment.Exit(1);
            }

            const string path = "/sys/class/dmi/id/product_serial";
            var serialNumber = File.ReadAllText(path);
            return serialNumber.Remove(serialNumber.Length - 1, 1);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Impossible de récupérer le numéro de série : {e.Message}");
            Environment.Exit(1);
            return null;
        }
    }

    public static void Main()
    {
        try
        {
            const string pathLicenseFile = "machine.lic";
            var licenseFileRaw = File.ReadAllText(pathLicenseFile);

            var serialNumber = GetSerialNumber();
            if (serialNumber is null)
            {
                Console.WriteLine(
                    "Unable to get serial number. Is your system compatible? Compatible systems list: [Windows, Linux]");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Serial number : " + serialNumber);
            }

            // Compute machine file fingerprint
            var hashAlgorithm = new Sha3Digest(512);
            var serialNumberBytes = Encoding.UTF8.GetBytes(serialNumber);
            hashAlgorithm.BlockUpdate(serialNumberBytes, 0, serialNumberBytes.Length);
            var result = new byte[hashAlgorithm.GetDigestSize()];
            hashAlgorithm.DoFinal(result, 0);
            var fingerprint = BitConverter.ToString(result);
            fingerprint = fingerprint.Replace("-", "").ToLower();

            // Parse signed license file (removing cert header, newlines and footer)
            string encodedPayload;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                encodedPayload = WindowsRegex().Replace(licenseFileRaw, "");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                encodedPayload = UnixRegex().Replace(licenseFileRaw, "");
            else
                encodedPayload = null;

            Debug.Assert(encodedPayload != null, nameof(encodedPayload) + " != null");
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
                // ReSharper disable once NotAccessedVariable
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

                        // secret is the hash of the license key and the machine fingerprint concatenated
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
                    // ReSharper disable once RedundantAssignment
                    plaintext = Encoding.UTF8.GetString(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to decrypt machine file: {e.Message}");

                    return;
                }

                Console.WriteLine("Machine file was successfully decrypted!");
                //Console.WriteLine($"Decrypted: {plaintext}"); // Uncomment to print decrypted data
            }
            else
            {
                Console.WriteLine("Invalid machine file!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to read machine file: {e.Message}");
            Environment.Exit(1);
        }

        Console.WriteLine("Hello, World!");
    }

    // Regex used for UNIX / OSX systems
    [GeneratedRegex("(^-----BEGIN MACHINE FILE-----\n|\n|-----END MACHINE FILE-----\n$)")]
    private static partial Regex UnixRegex();

    // Regex used for Windows systems
    [GeneratedRegex("(^-----BEGIN MACHINE FILE-----\r\n|\r\n|-----END MACHINE FILE-----\r\n$)")]
    private static partial Regex WindowsRegex();

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LicenseFile
    {
        public string enc { get; set; }
        public string sig { get; set; }
        public string alg { get; set; }
    }
}