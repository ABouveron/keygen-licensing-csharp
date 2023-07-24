namespace example_csharp_licensing_Docker.api.attributes;

public abstract class AuthenticationStrategy
{
    public const string Token = "TOKEN";
    public const string License = "LICENSE";
    public const string Mixed = "MIXED";
    public const string None = "NONE";
}