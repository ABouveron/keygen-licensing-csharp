namespace example_csharp_licensing_Docker.api.attributes;

public abstract class TransferStrategy
{
    public const string ResetExpiry = "RESET_EXPIRY";
    public const string KeepExpiry = "KEEP_EXPIRY";
}