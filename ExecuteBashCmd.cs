namespace example_csharp_licensing_Docker;

public class BashCmd
{
    public static string Execute(string command)
    {
        command = command.Replace("\"", "\"\"");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + command + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        Console.WriteLine("/bin/bash -c \"" + command + "\"");
        
        process.Start();
        process.WaitForExit();

        return process.StandardOutput.ReadToEnd();
    }
}