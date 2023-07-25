namespace example_csharp_licensing_Docker;

public abstract class Program
{
    public static async Task Main(string[]? args)
    {
        Env.Load();
        if (args == null || args.Length == 0)
            MachineFile.Verification(args);
        else
            switch (args[0])
            {
                case "license":
                    await CheckInternet.CheckInternetLicense(args[1..]);
                    return;
                case "api":
                    await CheckInternet.CheckInternetTest(args[1..]);
                    return;
                case "periodic":
                    await CheckInternet.CheckInternetPeriodic(args[1..]);
                    return;
                default:
                    MachineFile.Verification(args);
                    break;
            }
    }
}