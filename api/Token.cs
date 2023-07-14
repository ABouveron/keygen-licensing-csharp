namespace example_csharp_licensing_Docker.api;

public class Token
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
            var auth = new AuthPassword();
            password = auth.Password;
        }

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));

        var client = new RestClient(
            "https://api.keygen.localhost/v1/accounts/"
                + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("tokens", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader("Authorization", $"Basic {credentials}");

        return client.Execute(request);
    }
}
