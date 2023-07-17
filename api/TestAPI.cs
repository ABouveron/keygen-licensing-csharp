// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace example_csharp_licensing_Docker.api;

public abstract class TestApi
{
    public static void Main_aux()
    {
        Console.WriteLine(Token.GenAdminToken().Content);
        Console.WriteLine(
            "\nGet the admin token from the response above and put it in .env. You can now edit api/TestAPI.cs."
        );
        Console.WriteLine(
            "If there is no output and you are trying to contact localhost API, you probably did not execute the run script (you must execute install script before that)."
        );
    }
}