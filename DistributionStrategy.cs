namespace example_csharp_licensing_Docker;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class DistributionStrategy
{
    public static string Open { get; } = "OPEN";
    public static string Licensed { get; } = "LICENSED";
    public static string Closed { get; } = "CLOSED";
}