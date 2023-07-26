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
                case "testapi":
                    TestApi.MainApi(args[1..]);
                    return;
                case "license":
                    await CheckInternet.CheckInternetLicense(args[1..]);
                    return;
                case "api":
                    await CheckInternet.CheckInternetTest(args[1..]);
                    return;
                case "timecheck":
                    await CheckInternet.CheckInternetPeriodic(args[1..]);
                    return;
                case "fingerprint":
                    var serialNumber = SerialNumber.GetSerialNumber();
                    Debug.Assert(serialNumber != null, nameof(serialNumber) + " != null");
                    Console.WriteLine(SerialNumber.GetHash(serialNumber));
                    return;
                case "serialnumber":
                    Console.WriteLine(SerialNumber.GetSerialNumber());
                    return;
                default:
                    MachineFile.Verification(args);
                    break;
            }
    }
}