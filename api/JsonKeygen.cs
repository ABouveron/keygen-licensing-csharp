// ReSharper disable InconsistentNaming

namespace example_csharp_licensing_Docker.api;

public class JsonKeygen
{
    public Data? data;
}

public class Attributes
{
    public string? fingerprint;
    public string? name;
    public string? platform;
}

public class Data
{
    public Attributes? attributes;
    public Relationships? relationships;
    public string? type;
}

public class LicenseData
{
    public string? id;
    public string? type;
}

public class Relationships
{
    public License? license;
}
