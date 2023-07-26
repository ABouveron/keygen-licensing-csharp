// ReSharper disable InconsistentNaming

namespace example_csharp_licensing_Docker.api;

public class Attributes
{
    public int cores;
    public string? created;
    public bool encrypted;
    public string? expiry;

    // machine definition
    public string? fingerprint;
    public bool floating;
    public int heartbeatDuration;
    public string? heartbeatStatus;
    public string? hostname;
    public string? ip;
    public string? key;
    public string? lastCheckIn;
    public string? lastCheckOut;
    public string? lastHearbeat;
    public string? lastValidated;
    public int? maxCores;
    public int? maxMachines;
    public int? maxProcesses;
    public int? maxUses;

    public Dictionary<string, object>? metadata;

    // license definition
    public string? name;
    public string? nextCheckIn;
    public string? nextHeartbeat;
    public string[]? permissions;
    public string? platform;
    public bool @protected;
    public bool requireCheckIn;
    public bool requireHeatbeat;
    public string? scheme;
    public string? status;
    public bool strict;
    public bool suspended;
    public string? updated;
    public int? uses;
    public string? version;
}

public class Data
{
    public Attributes? attributes;
    public string? id;
    public Links? links;
    public Relationships? relationships;
    public string? type;
}

public class Links
{
    public string? related;
    public string? self;
}

public class Relationships
{
    // license definition
    public Account? account;
    public Entitlement? entitlements;
    public EnvironmentK? environment;
    public Group? group;

    // machine definition
    public License? license;
    public Machine? machines;
    public Policy? policy;
    public string[]? processes;
    public Product? product;
    public Token? tokens;
    public User? user;
}

public partial class Entitlement
{
    public Links? links;
}

public partial class Token
{
    public Data? data;
    public Links? links;
}

public partial class Machine
{
    public Meta? data;
    public Links? links;
}

public class Meta
{
    public int? cores;
    public int? count;
}

public partial class Group
{
    public Links? links;
}

public partial class User
{
    public Links? links;
}

public class EnvironmentK
{
    public Links? links;
}

public partial class Product
{
    public Data? data;
    public Links? links;
}

public class Account
{
    public Data? data;
    public Links? links;
}

public partial class License
{
    public Data? data;
}

public partial class Policy
{
    public string? authenticationStrategy;
    public string? checkInInterval;
    public int? checkInIntervalCount;
    public Data? data;
    public int? duration;
    public string? expirationBasis;
    public string? expirationStrategy;
    public string? fingerprintMatchingStrategy;
    public string? fingerprintUniquenessStrategy;
    public bool floating;
    public string? heartbeatBasis;
    public string? heartbeatCullStrategy;
    public int? heartbeatDuration;
    public string? heartbeatResurrectionStrategy;
    public string? leasingStrategy;
    public Links? links;
    public int? maxCores;
    public int? maxMachines;
    public int? maxProcesses;
    public int? maxUses;
    public string? name;
    public string? overageStrategy;
    public string? productId;
    public bool protectedParam;
    public bool requireCheckIn;
    public bool requireFingerprintScope;
    public bool requireHeartbeat;
    public bool requireMachineScope;
    public bool requirePolicyScope;
    public bool requireProductScope;
    public bool requireUserScope;
    public string? scheme;
    public bool strict;
    public string? transferStrategy;
    public bool usePool;
}