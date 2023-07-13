using Newtonsoft.Json;


namespace example_csharp_licensing_Docker;

public abstract class KeygenApiTest
{
    private static RestResponse GenAdminToken()
    {
        Console.WriteLine("Enter your Keygen account email:");
        var email = Console.ReadLine();
        Console.WriteLine("Enter your Keygen account password:");
        var auth = new AuthPassword();
        var password = auth.Password;
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));
        
        var client = new RestClient("https://api.keygen.sh/v1/accounts/" + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID"));
        var request = new RestRequest("tokens", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader("Authorization", $"Basic {credentials}");

        return client.Execute(request);
    }
    
    private static RestResponse GenEnvToken()
    {
        var client = new RestClient("https://api.keygen.sh/v1/accounts/" + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID"));

        var request = new RestRequest(
            "environments/b3ee7987-5309-4c61-9df1-c156a216db7a/tokens",
            Method.Post
        );
        
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader("Authorization", "Bearer " + Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN"));
     
        return client.Execute(request);
    }

    private static void LicenseAuth(string licenseId)
    {
        const string fingerprint = "PC_ABouveron";
        const string platform = "Linux";
        const string name = "ABouveron";
        const string licenseKey =
            "key/SlVFQy1YVzNFLVZYNEwtRUtXSy1VWEYzLUpNUkMtOUNIRi1FN0g5.RWwEs-2P3vOMZPIgoCdkPks6b_kkpp_yrZUmTB-pIXhcLvqYcJKOdI79oBu2qbQbwvrVFG77cSHeZZc1klT7Dg==";
        
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
                        data = new LicenseData
                        {
                            type = "licenses",
                            id = licenseId
                        }
                    }
                }
            }
        };
        Console.WriteLine(JsonConvert.SerializeObject(jsonLicenseKeyAuth));
        Console.WriteLine(BashCmd.Execute("curl -X POST https://api.keygen.sh/v1/accounts/" +
                                                Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID") + "/machines " +
                                                "-H 'Content-Type: application/vnd.api+json' " +
                                                "-H 'Accept: application/vnd.api+json' " +
                                                "-H 'Authorization: License " + licenseKey +  "' " +
                                                "-d '" + JsonConvert.SerializeObject(jsonLicenseKeyAuth) + "'"));
    }

    public static void ProductAuthentication(string licenseId)
    {
        const string fingerprint = "PC_ABouveron";
        const string platform = "Linux";
        const string name = "ABouveron";
        
        var jsonProdAuth = new JsonKeygen
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
                        data = new LicenseData
                        {
                            type = "licenses",
                            id = licenseId
                        }
                    }
                }
            }
        };
        Console.WriteLine(JsonConvert.SerializeObject(jsonProdAuth));
        Console.WriteLine(BashCmd.Execute("curl -X POST https://api.keygen.sh/v1/accounts/demo/machines " +
                                          "-H 'Content-Type: application/vnd.api+json' " +
                                          "-H 'Accept: application/vnd.api+json' " +
                                          "-H 'Authorization: Bearer " + Environment.GetEnvironmentVariable("KEYGEN_PRODUCT_TOKEN") +  "' " +
                                          "-d '" + JsonConvert.SerializeObject(jsonProdAuth) + "'"));
    }

    public static void Main(string[] args)
    {
        DotNetEnv.Env.Load();
        Console.WriteLine(GenAdminToken().Content);
        //const string licenseid = "651acd6a-0171-4b19-b4fd-0094a35a5186";
        //LicenseAuth(licenseid);
    }
}