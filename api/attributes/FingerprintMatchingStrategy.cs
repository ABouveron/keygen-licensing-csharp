namespace example_csharp_licensing_Docker.api.attributes;

public abstract class FingerprintMatchingStrategy
{
    public const string MatchAny = "MATCH_ANY";
    public const string MatchMost = "MATCH_MOST";
    public const string MatchAll = "MATCH_ALL";
}