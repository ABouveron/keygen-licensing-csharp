using System.Text.Json.Serialization;

namespace example_csharp_licensing_Docker.api;

public class LicenseActivation
{
    private RestClient client;

    public LicenseActivation(string accountId)
    {
        client = new RestClient($"https://api.keygen.sh/v1/accounts/{accountId}");
    }

    public async Task<Document<License, Validation>> ValidateLicense(
        string licenseKey,
        string deviceFingerprint
    )
    {
        var request = new RestRequest("licenses/actions/validate-key", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddJsonBody(
            new
            {
                meta = new { key = licenseKey, scope = new { fingerprint = deviceFingerprint, } }
            }
        );

        var response = await client.ExecuteAsync<Document<License, Validation>>(request);
        if ((response.Data ?? throw new Exception("response.Data null. License probably invalid")).Errors.Count > 0)
        {
            var err = response.Data.Errors[0];

            Console.WriteLine(
                "[ERROR] [ValidateLicense] Status={0} Title={1} Detail={2} Code={3}",
                response.StatusCode,
                err.Title,
                err.Detail,
                err.Code
            );

            System.Environment.Exit(1);
        }

        return response.Data;
    }

    public async Task<Document<Machine>> ActivateDevice(
        string licenseId,
        string licenseKey,
        string deviceFingerprint
    )
    {
        var request = new RestRequest("machines", Method.Post);

        request.AddHeader("Authorization", $"License {licenseKey}");
        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddJsonBody(
            new
            {
                data = new
                {
                    type = "machine",
                    attributes = new { fingerprint = deviceFingerprint, },
                    relationships = new
                    {
                        license = new { data = new { type = "license", id = licenseId, } }
                    }
                }
            }
        );
        
        var response = await client.ExecuteAsync<Document<Machine>>(request);
        if ((response.Data ?? throw new Exception("Invalid License")).Errors.Count > 0)
        {
            var err = response.Data.Errors[0];

            Console.WriteLine(
                "[ERROR] [ActivateDevice] Status={0} Title={1} Detail={2} Code={3}",
                response.StatusCode,
                err.Title,
                err.Detail,
                err.Code
            );

            System.Environment.Exit(1);
        }

        return response.Data;
    }

    public class Document<T>
    {
        public T? Data { get; set; }
        public List<Error> Errors { get; set; } = new();
    }

    public class Document<T, U> : Document<T>
    {
        public U? Meta { get; set; }
    }

    public class Error
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public string? Code { get; set; }
    }

    public class Validation
    {
        public Boolean Valid { get; set; }
        public string? Detail { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }

    public class License
    {
        public string? Type { get; set; }
        public string? ID { get; set; }
    }

    public class Machine
    {
        public string? Type { get; set; }
        public string? ID { get; set; }
    }
}

internal abstract class LicenseActivationAux
{
    private static async Task MainAsync(string[] args)
    {
        var keygen = new LicenseActivation(System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID") ?? 
                                           throw new Exception("KEYGEN_ACCOUNT_ID is null. Please set it in .env file."));

        // Keep a reference to the current license and device
        LicenseActivation.License license;
        LicenseActivation.Machine? device = null;

        // Validate license
        var validation = await keygen.ValidateLicense(
            args[0], // license key
            args[1] // device fingerprint
        );
        Console.WriteLine(
            (validation.Meta ?? throw new Exception("validation.Meta null. License probably invalid")).Valid
                ? "[INFO] [ValidateLicense] Valid={0} ValidationCode={1}"
                : "[INFO] [ValidateLicense] Invalid={0} ValidationCode={1}",
            validation.Meta.Detail,
            validation.Meta.Code
        );

        // Store license data
        license = validation.Data ?? throw new Exception("validation.Data null. License probably invalid");

        // Activate the current machine if it is not already activated (based on validation code)
        switch (validation.Meta.Code)
        {
            case "FINGERPRINT_SCOPE_MISMATCH":
            case "NO_MACHINES":
            case "NO_MACHINE":
                var activation = await keygen.ActivateDevice(
                    license.ID ?? throw new Exception("license.ID null. License probably invalid"),
                    args[0], // license key
                    args[1] // device fingerprint
                );

                // Store device data
                device = activation.Data;

                Console.WriteLine(
                    "[INFO] [ActivateDevice] DeviceId={0} LicenseId={1}",
                    (device ?? throw new Exception("device null. License probably invalid")).ID,
                    license.ID
                );

                // OPTIONAL: Validate license again
                validation = await keygen.ValidateLicense(
                    args[0], // license key
                    args[1] // device fingerprint
                );
                if ((validation.Meta ?? throw new Exception("validation.Meta null. License probably invalid")).Valid)
                {
                    Console.WriteLine(
                        "[INFO] [ValidateLicense] Valid={0} ValidationCode={1}",
                        validation.Meta.Detail,
                        validation.Meta.Code
                    );
                }
                else
                {
                    Console.WriteLine(
                        "[INFO] [ValidateLicense] Invalid={0} ValidationCode={1}",
                        validation.Meta.Detail,
                        validation.Meta.Code
                    );
                }

                break;
        }

        // Print the overall results
        Console.WriteLine(
            "[INFO] [Main] Valid={0} RecentlyActivated={1}",
            validation.Meta.Valid,
            device != null
        );

        if (validation.Meta.Valid)
        {
            Console.WriteLine("Hello, World!");
        }
    }

    public static void LicenseActivationMain(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }
}