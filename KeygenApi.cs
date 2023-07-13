using Newtonsoft.Json;

namespace example_csharp_licensing_Docker;

public abstract class KeygenApiTest
{
    private static RestResponse GenAdminToken()
    {
        var email = Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_EMAIL");
        var password = Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_PASSWORD");
        if (email == null || password == null)
        {
            Console.WriteLine("Enter your Keygen account email:");
            email = Console.ReadLine();
            Console.WriteLine("Enter your Keygen account password:");
            var auth = new AuthPassword();
            password = auth.Password;
        }
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));

        var client = new RestClient(
            "https://api.keygen.sh/v1/accounts/"
                + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("tokens", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader("Authorization", $"Basic {credentials}");

        return client.Execute(request);
    }

    private static void LicenseAuth(string licenseId, string licenseKey, string name, string platform, string fingerprint)
    {
        var jsonLicenseKeyAuth = new JsonKeygen
        {
            data = new Data
            {
                type = "machines",
                attributes = new Attributes
                {
                    fingerprint = fingerprint,
                    platform = platform,
                    name = name
                },
                relationships = new Relationships
                {
                    license = new License
                    {
                        data = new LicenseData { type = "licenses", id = licenseId }
                    }
                }
            }
        };
        Console.WriteLine(JsonConvert.SerializeObject(jsonLicenseKeyAuth));
        Console.WriteLine(
            BashCmd.Execute(
                "curl -X POST https://api.keygen.sh/v1/accounts/"
                    + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
                    + "/machines "
                    + "-H 'Content-Type: application/vnd.api+json' "
                    + "-H 'Accept: application/vnd.api+json' "
                    + "-H 'Authorization: License "
                    + licenseKey
                    + "' "
                    + "-d '"
                    + JsonConvert.SerializeObject(jsonLicenseKeyAuth)
                    + "'"
            )
        );
    }

    private static RestResponse ProductCreation(string name, string? url = null, string[]? platforms = null, string? distroStrat = null)
    {
        var client = new RestClient("https://api.keygen.sh/v1/accounts/" + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID"));
        var request = new RestRequest("products", Method.Post);
 
        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader("Authorization", "Bearer " + Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN"));

        distroStrat = distroStrat switch
        {
            "OPEN" => DistributionStrategy.Open,
            "CLOSED" => DistributionStrategy.Closed,
            _ => DistributionStrategy.Licensed
        };

        request.AddJsonBody(new {
            data = new {
                type = "products",
                attributes = new {
                    name,
                    url,
                    platforms,
                    distributionStrategy = distroStrat
                }
            }
        });
 
        return client.Execute(request);
    }

    public static void Main(string[] args)
    {
        DotNetEnv.Env.Load();
    }
}
