namespace example_csharp_licensing_Docker;

public abstract class Program
{
    public static async Task Main(string[]? args)
    {
        if (args == null || args.Length == 0)
            MachineFile.Verification(args);
        else
            switch (args[0])
            {
                case "api":
                    Env.Load();
                    await CheckInternet.CheckInternetAsync(args[1..]);
                    return;
                default:
                    MachineFile.Verification(args);
                    break;
            }
    }
}