using JsonSerializer = System.Text.Json.JsonSerializer;

namespace example_csharp_licensing_Docker;

public abstract partial class Program
{
    // Definition of all constant and variables needed to verify and decrypt the license file
    private static string _publicKey =
        "e8601e48b69383ba520245fd07971e983d06d22c4257cfd82304601479cee788";

    // Method to get EUID on Linux
    [DllImport("libc")]
    private static extern uint geteuid();

    // Method to get serial number of the machine
    private static string? GetSerialNumber()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS")
                    .Get()
                    .Cast<ManagementObject>()
                    .First()
                    .Properties["SerialNumber"].Value.ToString();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return null;
            if (geteuid() != 0)
            {
                Console.WriteLine(
                    "You must be root to get the serial number. Execute again with \"sudo dotnet run\"."
                );
                return null;
            }

            const string path = "/sys/class/dmi/id/product_serial";
            var serialNumber = File.ReadAllText(path);
            return serialNumber.Remove(serialNumber.Length - 1, 1);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Impossible de récupérer le numéro de série : {e.Message}");
            return null;
        }
    }

    public static void Main_aux(string[]? args)
    {
        try
        {
            string pathLicenseFile;
            string pathMachineFile;
            string fingerprint;

            if (args?.Length == 0)
            {
                pathLicenseFile = "license.lic";
                pathMachineFile = "machine.lic";

                var serialNumber = GetSerialNumber();
                if (serialNumber is null)
                {
                    Console.WriteLine(
                        "Unable to get serial number. Is your system compatible? Compatible systems list: [Windows, Linux]"
                    );
                    return;
                }

                Console.WriteLine("Serial number : " + serialNumber);

                // Compute machine file fingerprint
                var hashAlgorithm = new Sha3Digest(512);
                var serialNumberBytes = Encoding.UTF8.GetBytes(serialNumber);
                hashAlgorithm.BlockUpdate(serialNumberBytes, 0, serialNumberBytes.Length);
                var result = new byte[hashAlgorithm.GetDigestSize()];
                hashAlgorithm.DoFinal(result, 0);
                fingerprint = BitConverter.ToString(result);
                fingerprint = fingerprint.Replace("-", "").ToLower();

                Console.WriteLine("Replace \"PUBLIC_KEY\" line 96 with your public key (\"Ed25519 128-bit Verify Key\") available in https://app.keygen.sh/settings. Then comment lines 94 & 95 and run again.");
                return;  // Comment this line to continue
                _publicKey = "PUBLIC_KEY";
            }
            else
            {
                Debug.Assert(args != null, nameof(args) + " != null");
                pathLicenseFile = args[0];
                pathMachineFile = args[1];
                fingerprint = args[2];
            }

            var licenseKey = File.ReadAllText(pathLicenseFile);
            var machineFileRaw = File.ReadAllText(pathMachineFile);

            // Parse signed license file (removing cert header, newlines and footer)
            string? encodedPayload;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                encodedPayload = WindowsRegex().Replace(machineFileRaw, "");
            else if (
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            )
                encodedPayload = UnixRegex().Replace(machineFileRaw, "");
            else
                encodedPayload = null;

            Debug.Assert(encodedPayload != null, nameof(encodedPayload) + " != null");
            var payloadBytes = Convert.FromBase64String(encodedPayload);
            var payload = Encoding.UTF8.GetString(payloadBytes);
            string? encryptedData;
            string? encodedSignature;
            string? algorithm;

            // Deserialize license file certificate
            try
            {
                var lic = JsonSerializer.Deserialize<LicenseFile>(payload);

                Debug.Assert(lic != null, nameof(lic) + " != null");
                encryptedData = lic.enc;
                encodedSignature = lic.sig;
                algorithm = lic.alg;
            }
            catch (Exception e)
            {
                Console.WriteLine((object?)$"Failed to parse machine file: {e.Message}");

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
            Debug.Assert(encodedSignature != null, nameof(encodedSignature) + " != null");
            var signatureBytes = Convert.FromBase64String(encodedSignature);
            var signingDataBytes = Encoding.UTF8.GetBytes($"machine/{encryptedData}");
            var publicKeyBytes = Convert.FromHexString(_publicKey);
            var key = PublicKey.Import(ed25519, publicKeyBytes, KeyBlobFormat.RawPublicKey);

            if (ed25519.Verify(key, signingDataBytes, signatureBytes))
            {
                Console.WriteLine("Machine file is valid! Decrypting...");

                // Decrypt license file dataset
                // ReSharper disable once NotAccessedVariable
                string plaintext;
                try
                {
                    Debug.Assert(encryptedData != null, nameof(encryptedData) + " != null");
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
                        var licenseKeyBytes = Encoding.UTF8.GetBytes(licenseKey);
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
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to read machine file: {e.Message}");
            return;
        }

        Console.WriteLine("Hello, World!");
    }

    // Regex used for UNIX / OSX systems
    [GeneratedRegex("(^-----BEGIN MACHINE FILE-----\n|\n|-----END MACHINE FILE-----\n$)")]
    private static partial Regex UnixRegex();

    // Regex used for Windows systems
    [GeneratedRegex(
        "(^-----BEGIN MACHINE FILE-----\n|^-----BEGIN MACHINE FILE-----\r\n|\r\n|-----END MACHINE FILE-----\n$|-----END MACHINE FILE-----\r\n$)"
    )]
    private static partial Regex WindowsRegex();

    public static async Task Main(string[]? args)
    {
        if (args == null || args.Length == 0)
        {
            Main_aux(args);
        }
        else
        {
            if (args[0] == "api")
            {
                Env.Load();
                await CheckInternet.CheckInternetAsync();
                TestApi.Main_aux();
                return;
            }

            Main_aux(args);
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LicenseFile
    {
        public string? enc { get; set; }
        public string? sig { get; set; }
        public string? alg { get; set; }
    }
}