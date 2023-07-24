namespace example_csharp_licensing_Docker.api.attributes;

public abstract class LeasingStrategy
{
    public const string PerMachine = "PER_MACHINE";
    public const string PerLicense = "PER_LICENSE";
}