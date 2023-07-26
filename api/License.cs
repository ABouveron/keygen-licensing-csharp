namespace example_csharp_licensing_Docker.api;

public partial class License
{
    public int GetUses()
    {
        return data?.attributes?.uses ?? 0;
    }

    public string GetKey()
    {
        return data?.attributes?.key ?? "";
    }

    public static RestResponse Creation()
    {
        var client = new RestClient(
            "https://api.keygen.sh/v1/accounts/"
            + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("licenses", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + System.Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        request.AddJsonBody(
            new
            {
                data = new
                {
                    type = "licenses",
                    attributes = new
                    {
                        name = "test license api",
                        expiry = "2023-07-25T06:57:00.000Z"
                    },
                    relationships = new
                    {
                        policy = new
                        {
                            data = new
                            {
                                type = "policies",
                                id = "43c66056-d6d4-437a-9d9c-522d8873a38f"
                            }
                        },
                        user = new
                        {
                            data = new
                            {
                                type = "users",
                                id = "e87cff84-411f-48aa-97c3-52ecb55cd644"
                            }
                        }
                    }
                }
            }
        );

        return client.Execute(request);
    }

    public static License Retrieve(string licenseId)
    {
        var client = new RestClient(
            "https://api.keygen.sh/v1/accounts/"
            + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("licenses/" + licenseId);

        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + System.Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        var response = client.Execute(request);
        var license = JsonToLicense(
            response.Content ?? throw new InvalidOperationException("response.Content == null")
        );
        return license;
    }

    public void PrintInfos()
    {
        Console.WriteLine("Uses = " + GetUses());
        Console.WriteLine("Key = " + GetKey());
    }

    private static License JsonToLicense(string json)
    {
        var license =
            JsonConvert.DeserializeObject<License>(json)
            ?? throw new InvalidOperationException(
                "JsonConvert.DeserializeObject<License>(json) == null"
            );
        return license;
    }
}