namespace example_csharp_licensing_Docker.api;

public class Entitlement
{
    public static RestResponse EntitlementCreation(string name, string code)
    {
        var client = new RestClient(
            "https://api.keygen.localhost/v1/accounts/"
                + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("entitlements", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + System.Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        request.AddJsonBody(
            new { data = new { type = "entitlements", attributes = new { name, code } } }
        );

        return client.Execute(request);
    }
}
