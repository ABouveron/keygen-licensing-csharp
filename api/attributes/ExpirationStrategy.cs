namespace example_csharp_licensing_Docker.api.attributes;

public abstract class ExpirationStrategy
{
    public const string RestrictAccess = "RESTRICT_ACCESS";
    public const string RevokeAccess = "REVOKE_ACCESS";
    public const string MaintainAccess = "MAINTAIN_ACCESS";
    public const string AllowAccess = "ALLOW_ACCESS";
}