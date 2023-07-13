namespace example_csharp_licensing_Docker;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class DistributionStrategy
{
    public const string Open = "OPEN";
    public const string Licensed = "LICENSED";
    public const string Closed = "CLOSED";
}
