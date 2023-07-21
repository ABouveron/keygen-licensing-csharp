namespace example_csharp_licensing_Docker.api;

public abstract class Authentication
{
    public static void LicenseAuth(
        string licenseId,
        string licenseKey,
        string name,
        string platform,
        string fingerprint
    )
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
        Console.WriteLine(
            BashCmd.Execute(
                "curl -X POST https://"
                + CheckInternet.api
                + "/v1/accounts/"
                + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
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
}