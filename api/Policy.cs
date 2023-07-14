using example_csharp_licensing_Docker.api.attributes;

namespace example_csharp_licensing_Docker.api;

public class Policy
{
    public static RestResponse PolicyCreation(
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
            "https://"
            + CheckInternet.Api
            + "/v1/accounts/"
            + System.Environment.GetEnvironmentVariable("KEYGEN_ACCOUNT_ID")
        );
        var request = new RestRequest("policies", Method.Post);

        request.AddHeader("Content-Type", "application/vnd.api+json");
        request.AddHeader("Accept", "application/vnd.api+json");
        request.AddHeader(
            "Authorization",
            "Bearer " + System.Environment.GetEnvironmentVariable("KEYGEN_ADMIN_TOKEN")
        );

        scheme = scheme switch
        {
            "RSA_2048_PKCS1_PSS_SIGN_V2" => Scheme.Rsa2048Pkcs1PssSignV2,
            "RSA_2048_PKCS1_SIGN_V2" => Scheme.Rsa2048Pkcs1SignV2,
            "RSA_2048_PKCS1_ENCRYPT" => Scheme.Rsa2048Pkcs1Encrypt,
            "RSA_2048_JWT_RS256" => Scheme.Rsa2048JwtRs256,
            _ => Scheme.Ed25519Sign
        };

        checkInInterval = checkInInterval switch
        {
            "week" => CheckInInterval.Week,
            "month" => CheckInInterval.Month,
            "year" => CheckInInterval.Year,
            _ => CheckInInterval.Day
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
                        product = new { data = new { type = "product", id = productId } }
                    }
                }
            }
        );

        return client.Execute(request);
    }
}