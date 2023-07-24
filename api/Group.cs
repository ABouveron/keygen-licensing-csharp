namespace example_csharp_licensing_Docker.api;

public class Group
{
    public static RestResponse GroupCreation(
        string name,
        int maxUsers = default,
        int maxLicenses = default,
        int maxMachines = default
    )
    {
        var client = new RestClient(
            "https://"
            + CheckInternet.api
            + "/v1/accounts/"
            + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("groups", Method.Post);

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
                    type = "groups",
                    attributes = new
                    {
                        name,
                        maxUsers,
                        maxLicenses,
                        maxMachines
                    }
                }
            }
        );

        return client.Execute(request);
    }
}