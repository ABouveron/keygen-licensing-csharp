// ReSharper disable InconsistentNaming
namespace example_csharp_licensing_Docker;

    
public class JsonKeygen
{
    public Data? data;
}

public class Attributes
{
    public string? fingerprint;
    public string? platform;
    public string? name;
}

public class Data
{
    public string? type;
    public Attributes? attributes;
    public Relationships? relationships;
}

public class LicenseData
{
    public string? type;
    public string? id;
}

public class License
{
    public LicenseData? data;
}

public class Relationships
{
    public License? license;
}
