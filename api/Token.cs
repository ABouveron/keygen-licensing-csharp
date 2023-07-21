namespace example_csharp_licensing_Docker.api;

public abstract class Token
{
    public static RestResponse GenAdminToken()
    {
        var email = System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_EMAIL");
        var password = System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_PASSWORD");
        if (email == null || password == null)
        {
            Console.WriteLine("Enter your Keygen account email:");
            email = Console.ReadLine();
            Console.WriteLine("Enter your Keygen account password:");
            password = AuthPassword.Password;
        }

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));

        var client = new RestClient(
            "https://"
            + CheckInternet.api
            + "/v1/accounts/"
            + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("tokens", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader("Authorization", $"Basic {credentials}");

        return client.Execute(request);
    }
}