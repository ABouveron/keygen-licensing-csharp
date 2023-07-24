namespace example_csharp_licensing_Docker.utilities;

public abstract class SerialNumber
{
    // Method to get EUID on Linux
    [DllImport("libc")]
    private static extern uint geteuid();

    public static string? GetSerialNumber()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS")
                    .Get()
                    .Cast<ManagementObject>()
                    .First()
                    .Properties["SerialNumber"].Value.ToString();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return null;
            if (geteuid() != 0)
            {
                Console.WriteLine(
                    "You must be root to get the serial number. Execute again with \"sudo dotnet run\"."
                );
                return null;
            }

            const string path = "/sys/class/dmi/id/product_serial";
            var serialNumber = File.ReadAllText(path);
            return serialNumber.Remove(serialNumber.Length - 1, 1);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Impossible de récupérer le numéro de série : {e.Message}");
            return null;
        }
    }
}