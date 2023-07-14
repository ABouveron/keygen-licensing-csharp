using Process = System.Diagnostics.Process;

namespace example_csharp_licensing_Docker.utilities;

public abstract class BashCmd
{
    public static string Execute(string command)
    {
        command = command.Replace("\"", "\"\"");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = "-c \"" + command + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();

        return process.StandardOutput.ReadToEnd();
    }
}