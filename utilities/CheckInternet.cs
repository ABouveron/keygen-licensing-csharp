namespace example_csharp_licensing_Docker.utilities;

public abstract class CheckInternet
{
    public static string api = "api.keygen.sh";

    public static async Task CheckInternetAsync(string[] args)
    {
        var myPing = new Ping();
        try
        {
            await myPing.SendPingAsync("google.com", 3000, new byte[32], new PingOptions(64, true));
            // use official API api.keygen.sh
            Console.WriteLine("Using official API...");
            api = "api.keygen.sh";
            TestApi.MainApi();
        }
        catch (Exception)
        {
            Console.WriteLine(
                "Using official API did not succeed...\n" + "Now trying to use machine file..."
            );
            MachineFile.Verification(args);
        }
    }
}