using DotNetEnv;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace example_csharp_licensing_Docker.api;

public abstract class KeygenApi
{
    private static RestResponse GenAdminToken()
    {
        var email = Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_EMAIL");
        var password = Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_PASSWORD");
        if (email == null || password == null)
        {
            Console.WriteLine("Enter your Keygen account email:");
            email = Console.ReadLine();
            Console.WriteLine("Enter your Keygen account password:");
            var auth = new AuthPassword();
            password = auth.Password;
        }

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));

        var client = new RestClient(
            "https://api.keygen.sh/v1/accounts/"
                + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("tokens", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader("Authorization", $"Basic {credentials}");

        return client.Execute(request);
    }

    private static void LicenseAuth(
        string licenseId,
        string licenseKey,
        string name,
        string platform,
        string fingerprint
    )
    {
        var jsonLicenseKeyAuth = new JsonKeygen
        {
            data = new Data
            {
                type = "machines",
                attributes = new Attributes
                {
                    fingerprint = fingerprint,
                    platform = platform,
                    name = name
                },
                relationships = new Relationships
                {
                    license = new License
                    {
                        data = new LicenseData { type = "licenses", id = licenseId }
                    }
                }
            }
        };
        Console.WriteLine(JsonConvert.SerializeObject(jsonLicenseKeyAuth));
        Console.WriteLine(
            BashCmd.Execute(
                "curl -X POST https://api.keygen.sh/v1/accounts/"
                    + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
                    + "/machines "
                    + "-H 'Content-Type: application/vnd.api+json' "
                    + "-H 'Accept: application/vnd.api+json' "
                    + "-H 'Authorization: License "
                    + licenseKey
                    + "' "
                    + "-d '"
                    + JsonConvert.SerializeObject(jsonLicenseKeyAuth)
                    + "'"
            )
        );
    }

    private static RestResponse ProductCreation(
        string name,
        string? url = default,
        string[]? platforms = default,
        string? distroStrat = default
    )
    {
        var client = new RestClient(
            "https://api.keygen.sh/v1/accounts/"
                + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("products", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        distroStrat = distroStrat switch
        {
            "OPEN" => DistributionStrategy.Open,
            "CLOSED" => DistributionStrategy.Closed,
            _ => DistributionStrategy.Licensed
        };

        request.AddJsonBody(
            new
            {
                data = new
                {
                    type = "products",
                    attributes = new
                    {
                        name,
                        url,
                        platforms,
                        distributionStrategy = distroStrat
                    }
                }
            }
        );

        return client.Execute(request);
    }

    private static RestResponse EntitlementCreation(string name, string code)
    {
        var client = new RestClient(
            "https://api.keygen.sh/v1/accounts/"
                + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("entitlements", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        request.AddJsonBody(
            new { data = new { type = "entitlements", attributes = new { name, code } } }
        );

        return client.Execute(request);
    }

    private static RestResponse GroupCreation(
        string name,
        int maxUsers = default,
        int maxLicenses = default,
        int maxMachines = default
    )
    {
        var client = new RestClient(
            "https://api.keygen.sh/v1/accounts/"
                + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("groups", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        request.AddJsonBody(
            new
            {
                data = new
                {
                    type = "groups",
                    attributes = new
                    {
                        name,
                        maxUsers,
                        maxLicenses,
                        maxMachines
                    }
                }
            }
        );

        return client.Execute(request);
    }

    private static RestResponse PolicyCreation(
        string name,
        string productId,
        int duration = 86400,
        string? scheme = default,
        bool strict = default,
        bool floating = default,
        bool requireProductScope = default,
        bool requirePolicyScope = default,
        bool requireMachineScope = default,
        bool requireFingerprintScope = default,
        bool requireUserScope = default,
        bool requireCheckIn = default,
        string? checkInInterval = default,
        int checkInIntervalCount = default,
        bool usePool = default,
        int maxMachines = 1,
        int maxProcesses = 1,
        int maxCores = 1,
        int maxUses = default,
        bool protectedParam = default,
        bool requireHeartbeat = default,
        int heartbeatDuration = 60,
        string? heartbeatCullStrategy = default,
        string? heartbeatResurrectionStrategy = default,
        string? heartbeatBasis = default,
        string? fingerprintUniquenessStrategy = default,
        string? fingerprintMatchingStrategy = default,
        string? expirationStrategy = default,
        string? expirationBasis = default,
        string? transferStrategy = default,
        string? authenticationStrategy = default,
        string? overageStrategy = default,
        string? leasingStrategy = default
    )
    {
        var client = new RestClient(
            "https://api.keygen.sh/v1/accounts/"
                + Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("policies", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        scheme = scheme switch
        {
            "RSA_2048_PKCS1_PSS_SIGN_V2" => Scheme.Rsa2048Pkcs1PssSignV2,
            "RSA_2048_PKCS1_SIGN_V2" => Scheme.Rsa2048Pkcs1PssSignV2,
            "RSA_2048_PKCS1_ENCRYPT" => Scheme.Rsa2048Pkcs1Encrypt,
            "RSA_2048_JWT_RS256" => Scheme.Rsa2048JwtRs256,
            _ => Scheme.Ed25519Sign
        };

        checkInInterval = checkInInterval switch
        {
            "week" => ChekInInterval.Week,
            "month" => ChekInInterval.Month,
            "year" => ChekInInterval.Year,
            _ => ChekInInterval.Day
        };

        heartbeatCullStrategy = heartbeatCullStrategy switch
        {
            "KEEP_DEAD" => HeartbeatCullStrategy.KeepDead,
            _ => HeartbeatCullStrategy.DeactivateDead
        };

        heartbeatResurrectionStrategy = heartbeatResurrectionStrategy switch
        {
            "1_MINUTE_REVIVE" => HeartbeatResurrectionStrategy.OneMinuteRevive,
            "2_MINUTE_REVIVE" => HeartbeatResurrectionStrategy.TwoMinutesRevive,
            "5_MINUTE_REVIVE" => HeartbeatResurrectionStrategy.FiveMinutesRevive,
            "10_MINUTE_REVIVE" => HeartbeatResurrectionStrategy.TenMinutesRevive,
            "15_MINUTE_REVIVE" => HeartbeatResurrectionStrategy.FifteenMinutesRevive,
            _ => HeartbeatResurrectionStrategy.NoRevive
        };

        heartbeatBasis = heartbeatBasis switch
        {
            "FROM_FIRST_PING" => HeartbeatBasis.FromFirstPing,
            _ => HeartbeatBasis.FromCreation
        };

        fingerprintUniquenessStrategy = fingerprintUniquenessStrategy switch
        {
            "UNIQUE_PER_PRODUCT" => FingerprintUniquenessStrategy.UniquePerProduct,
            "UNIQUE_PER_POLICY" => FingerprintUniquenessStrategy.UniquePerPolicy,
            "UNIQUE_PER_LICENSE" => FingerprintUniquenessStrategy.UniquePerLicense,
            _ => FingerprintUniquenessStrategy.UniquePerAccount
        };

        fingerprintMatchingStrategy = fingerprintMatchingStrategy switch
        {
            "MATCH_MOST" => FingerprintMatchingStrategy.MatchMost,
            "MATCH_ALL" => FingerprintMatchingStrategy.MatchAll,
            _ => FingerprintMatchingStrategy.MatchAny
        };

        expirationStrategy = expirationStrategy switch
        {
            "REVOKE_ACCESS" => ExpirationStrategy.RevokeAccess,
            "MAINTAIN_ACCESS" => ExpirationStrategy.MaintainAccess,
            "ALLOW_ACCESS" => ExpirationStrategy.AllowAccess,
            _ => ExpirationStrategy.RestrictAccess
        };

        expirationBasis = expirationBasis switch
        {
            "FROM_FIRST_ACTIVATION" => ExpirationBasis.FromFirstActivation,
            "FROM_FIRST_VALIDATION" => ExpirationBasis.FromFirstValidation,
            "FROM_FIRST_USE" => ExpirationBasis.FromFirstUse,
            "FROM_FIRST_DOWNLOAD" => ExpirationBasis.FromFirstDownload,
            _ => ExpirationBasis.FromCreation
        };

        transferStrategy = transferStrategy switch
        {
            "RESET_EXPIRY" => TransferStrategy.ResetExpiry,
            _ => TransferStrategy.KeepExpiry
        };

        authenticationStrategy = authenticationStrategy switch
        {
            "LICENSE" => AuthenticationStrategy.License,
            "MIXED" => AuthenticationStrategy.Mixed,
            "NONE" => AuthenticationStrategy.None,
            _ => AuthenticationStrategy.Token
        };

        leasingStrategy = leasingStrategy switch
        {
            "PER_LICENSE" => LeasingStrategy.PerLicense,
            _ => LeasingStrategy.PerMachine
        };

        overageStrategy = overageStrategy switch
        {
            "ALWAYS_ALLOW_OVERAGE" => OverageStrategy.AlwaysAllowOverage,
            "ALLOW_1_25X_OVERAGE" => OverageStrategy.AllowOne25TimesOverage,
            "ALLOW_1_5X_OVERAGE" => OverageStrategy.AllowOne5TimesOverage,
            "ALLOW_2X_OVERAGE" => OverageStrategy.Allow2TimesOverage,
            _ => OverageStrategy.NoOverage
        };

        request.AddJsonBody(
            new
            {
                data = new
                {
                    type = "policies",
                    attributes = new
                    {
                        name,
                        scheme,
                        duration,
                        strict,
                        floating,
                        requireProductScope,
                        requirePolicyScope,
                        requireMachineScope,
                        requireFingerprintScope,
                        requireUserScope,
                        requireCheckIn,
                        checkInInterval,
                        checkInIntervalCount,
                        usePool,
                        maxMachines,
                        maxProcesses,
                        maxCores,
                        maxUses,
                        @protected = protectedParam,
                        requireHeartbeat,
                        heartbeatDuration,
                        heartbeatCullStrategy,
                        heartbeatResurrectionStrategy,
                        heartbeatBasis,
                        fingerprintUniquenessStrategy,
                        fingerprintMatchingStrategy,
                        expirationStrategy,
                        expirationBasis,
                        transferStrategy,
                        authenticationStrategy,
                        leasingStrategy,
                        overageStrategy
                    },
                    relationships = new
                    {
                        product = new
                        {
                            data = new
                            {
                                type = "product",
                                id = productId
                            }
                        }
                    }
                }
            }
        );

        return client.Execute(request);
    }

    public static void Main(string[] args)
    {
        Env.Load();
        Console.WriteLine(PolicyCreation("test", "968914f2-a73d-4c56-89f5-95234e2a3292").Content);
    }
}
