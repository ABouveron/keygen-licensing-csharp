namespace example_csharp_licensing_Docker.api.attributes;

public abstract class HeartbeatCullStrategy
{
    public const string DeactivateDead = "DEACTIVATE_DEAD";
    public const string KeepDead = "KEEP_DEAD";
}