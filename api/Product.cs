namespace example_csharp_licensing_Docker.api;

public class Product
{
    public static RestResponse ProductCreation(
        string name,
        string? url = default,
        string[]? platforms = default,
        string? distroStrat = default
    )
    {
        var client = new RestClient(
            "https://api.keygen.localhost/v1/accounts/"
                + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("products", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + System.Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        distroStrat = distroStrat switch
        {
            "OPEN" => DistributionStrategy.Open,
            "CLOSED" => DistributionStrategy.Closed,
            _ => DistributionStrategy.Licensed
        };

        request.AddJsonBody(
            new
            {
                data = new
                {
                    type = "products",
                    attributes = new
                    {
                        name,
                        url,
                        platforms,
                        distributionStrategy = distroStrat
                    }
                }
            }
        );

        return client.Execute(request);
    }
}
