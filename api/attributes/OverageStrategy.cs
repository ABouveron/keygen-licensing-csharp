namespace example_csharp_licensing_Docker.api.attributes;

public abstract class OverageStrategy
{
    public const string AlwaysAllowOverage = "ALWAYS_ALLOW_OVERAGE";
    public const string AllowOne25TimesOverage = "ALLOW_ONE_25X_OVERAGE";
    public const string AllowOne5TimesOverage = "ALLOW_ONE_5X_OVERAGE";
    public const string Allow2TimesOverage = "ALLOW_2X_OVERAGE";
    public const string NoOverage = "NO_OVERAGE";
}