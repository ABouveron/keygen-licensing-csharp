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
                case "offlineapi":
                    Env.Load();
                    await CheckInternet.GetOfflineApi();
                    TestApi.MainApi();
                    return;
                case "api":
                    Env.Load();
                    await CheckInternet.CheckInternetAsync(args);
                    TestApi.MainApi();
                    return;
                case "obfuscation":
                    await Main(args[1..]);
                    const string reactorPath = @"./dotNET_Reactor";
                    const string commandline =
                        @" -quiet -file ""./bin/Debug/net7.0/example-csharp-licensing-Docker.dll"" -antitamp 1 -anti_debug 1 -hide_calls 1 -control_flow 1 -flow_level 9 -resourceencryption 1 -virtualization 1 -necrobit 1 -mapping_file 1 -rules "".*::assemblies:^example-csharp-licensing-Docker.dll$::types:^Target$::where:class""";
                    BashCmd.Execute(reactorPath + commandline);
                    return;
                default:
                    MachineFile.Verification(args);
                    break;
            }
    }
}