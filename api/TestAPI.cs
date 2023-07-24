// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace example_csharp_licensing_Docker.api;

public abstract class TestApi
{
    public static void MainApi()
    {
        Console.WriteLine(Token.GenAdminToken().Content);
        Console.WriteLine(
            "\nGet the admin token from the response above and put it in .env. You can now edit api/TestAPI.cs."
        );
    }
}