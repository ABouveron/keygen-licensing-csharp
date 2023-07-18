namespace example_csharp_licensing_Docker.api.attributes;

public abstract class ExpirationBasis
{
    public const string FromCreation = "FROM_CREATION";
    public const string FromFirstValidation = "FROM_FIRST_VALIDATION";
    public const string FromFirstActivation = "FROM_FIRST_ACTIVATION";
    public const string FromFirstDownload = "FROM_FIRST_DOWNLOAD";
    public const string FromFirstUse = "FROM_FIRST_USE";
}