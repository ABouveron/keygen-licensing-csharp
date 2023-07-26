namespace example_csharp_licensing_Docker.utilities;

public abstract class CheckInternet
{
    public static string api = "api.keygen.sh";

    public static async Task CheckInternetTest(string[] args)
    {
        var myPing = new Ping();
        try
        {
            await myPing.SendPingAsync("google.com", 3000, new byte[32], new PingOptions(64, true));
            // use official API api.keygen.sh
            Console.WriteLine("Using official API...");
            api = "api.keygen.sh";
            TestApi.MainApi(args);
        }
        catch (Exception)
        {
            Console.WriteLine(
                "Using official API did not succeed...\n" + "Now trying to use machine file..."
            );
            MachineFile.Verification(args);
        }
    }

    public static async Task CheckInternetLicense(string[] args)
    {
        var myPing = new Ping();
        try
        {
            await myPing.SendPingAsync("google.com", 3000, new byte[32], new PingOptions(64, true));

            // use official API api.keygen.sh
            Console.WriteLine("Using official API...");
            api = "api.keygen.sh";
            LicenseActivationAux.LicenseActivationMain(args);
        }
        catch (Exception)
        {
            Console.WriteLine(
                "Using official API did not succeed...\nNow trying to use machine file..."
            );
            MachineFile.Verification(args.Length < 2 ? args : args[2..]);
        }
    }

    public static async Task CheckInternetPeriodic(string[] args)
    {
        await PeriodicCheck(args[1..], TimeSpan.FromSeconds(int.Parse(args[0])));
    }

    private static async Task PeriodicCheck(
        string[] args,
        TimeSpan interval,
        CancellationToken cancellationToken = default
    )
    {
        while (true)
        {
            await CheckInternetLicense(args);
            await Task.Delay(interval, cancellationToken);
        }
    }
}