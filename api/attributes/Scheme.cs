namespace example_csharp_licensing_Docker;

public abstract class Scheme
{
    public const string Ed25519Sign = "ED25519_SIGN";
    public const string Rsa2048Pkcs1PssSignV2 = "RSA_2048_PKCS1_PSS_SIGN_V2";
    public const string Rsa2048Pkcs1SignV2 = "RSA_2048_PKCS1_SIGN_V2";
    public const string Rsa2048Pkcs1Encrypt = "RSA_2048_PKCS1_ENCRYPT";
    public const string Rsa2048JwtRs256 = "RSA_2048_JWT_RS256";
}
