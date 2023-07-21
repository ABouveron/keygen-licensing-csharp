using static System.Environment;

namespace example_csharp_licensing_Docker.utilities;

public abstract class CheckInternet
{
    public static string api = "api.keygen.sh";

    public static async Task CheckInternetAsync()
    {
        var myPing = new Ping();
        try
        {
            await myPing.SendPingAsync("google.com", 3000, new byte[32], new PingOptions(64, true));
            // use official API api.keygen.sh
            Console.WriteLine("Using official API...");
            api = "api.keygen.sh";
            SetEnvironmentVariable(
                "KEYGEN_ACCOUNT_ID",
                GetEnvironmentVariable("KEYGEN_ACCOUNT_ID_OFFICIAL")
            );
            SetEnvironmentVariable(
                "KEYGEN_ACCOUNT_EMAIL",
                GetEnvironmentVariable("KEYGEN_ACCOUNT_EMAIL_OFFICIAL")
            );
            SetEnvironmentVariable(
                "KEYGEN_ACCOUNT_PASSWORD",
                GetEnvironmentVariable("KEYGEN_ACCOUNT_PASSWORD_OFFICIAL")
            );
            SetEnvironmentVariable(
                "KEYGEN_ADMIN_TOKEN",
                GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN_OFFICIAL")
            );
        }
        catch (Exception)
        {
            // use localhost API api.keygen.localhost
            Console.WriteLine("Switching to localhost API...");
            api = "api.keygen.localhost";
            SetEnvironmentVariable(
                "KEYGEN_ACCOUNT_ID",
                GetEnvironmentVariable("KEYGEN_ACCOUNT_ID_LOCALHOST")
            );
            SetEnvironmentVariable(
                "KEYGEN_ACCOUNT_EMAIL",
                GetEnvironmentVariable("KEYGEN_ACCOUNT_EMAIL_LOCALHOST")
            );
            SetEnvironmentVariable(
                "KEYGEN_ACCOUNT_PASSWORD",
                GetEnvironmentVariable("KEYGEN_ACCOUNT_PASSWORD_LOCALHOST")
            );
            SetEnvironmentVariable(
                "KEYGEN_ADMIN_TOKEN",
                GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN_LOCALHOST")
            );
        }
    }

    public static Task GetOfflineApi()
    {
        // use localhost API api.keygen.localhost
        Console.WriteLine("Switching to localhost API...");
        api = "api.keygen.localhost";
        SetEnvironmentVariable(
            "KEYGEN_ACCOUNT_ID",
            GetEnvironmentVariable("KEYGEN_ACCOUNT_ID_LOCALHOST")
        );
        SetEnvironmentVariable(
            "KEYGEN_ACCOUNT_EMAIL",
            GetEnvironmentVariable("KEYGEN_ACCOUNT_EMAIL_LOCALHOST")
        );
        SetEnvironmentVariable(
            "KEYGEN_ACCOUNT_PASSWORD",
            GetEnvironmentVariable("KEYGEN_ACCOUNT_PASSWORD_LOCALHOST")
        );
        SetEnvironmentVariable(
            "KEYGEN_ADMIN_TOKEN",
            GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN_LOCALHOST")
        );
        return Task.CompletedTask;
    }
}