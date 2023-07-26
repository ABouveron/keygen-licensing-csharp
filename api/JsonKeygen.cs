// ReSharper disable InconsistentNaming

namespace example_csharp_licensing_Docker.api;

public class Attributes
{
    // license definition
    public string? name;
    public string? key;
    public string? expiry;
    public string? status;
    public int? uses;
    public bool @protected;
    public string? version;
    public bool suspended;
    public bool encrypted;
    public bool floating;
    public string? scheme;
    public bool strict;
    public int? maxMachines;
    public int? maxProcesses;
    public int? maxCores;
    public int? maxUses;
    public bool requireHeatbeat;
    public bool requireCheckIn;
    public string? lastValidated;
    public string? lastCheckOut;
    public string? lastCheckIn;
    public string? nextCheckIn;
    public string[]? permissions;
    public Dictionary<string, object>? metadata;
    public string? created;
    public string? updated;
    
    // machine definition
    public string? fingerprint;
    public int cores;
    public string? ip;
    public string? hostname;
    public string? platform;
    public string? heartbeatStatus;
    public int heartbeatDuration;
    public string? lastHearbeat;
    public string? nextHeartbeat;
}

public class Data
{
    public string? id;
    public string? type;
    public Attributes? attributes;
    public Relationships? relationships;
    public Links? links;
}

public class Links
{
    public string? self;
    public string? related;
}

public class Relationships
{
    // license definition
    public Account? account;
    public Product? product;
    public Policy? policy;
    public Group? group;
    public User? user;
    public Machine? machines;
    public EnvironmentK? environment;
    public Token? tokens;
    public Entitlement? entitlements;
    
    // machine definition
    public License? license;
    public string[]? processes;
}

public partial class Entitlement
{
    public Links? links;
}

public partial class Token
{
    public Links? links;
    public Data? data;
}

public partial class Machine
{
    public Links? links;
    public Meta? data;
}

public class Meta
{
    public int? cores;
    public int? count;
}

public partial class Group{
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
    public Links? links;
    public Data? data;
}

public class Account
{
    public Links? links;
    public Data? data;
}

public partial class License
{
    public Data? data;
}

public partial class Policy
{
    public string? name;
    public string? productId;
    public int? duration;
    public string? scheme;
    public bool strict;
    public bool floating;
    public bool requireProductScope;
    public bool requirePolicyScope;
    public bool requireMachineScope;
    public bool requireFingerprintScope;
    public bool requireUserScope;
    public bool requireCheckIn;
    public string? checkInInterval;
    public int? checkInIntervalCount;
    public bool usePool;
    public int? maxMachines;
    public int? maxProcesses;
    public int? maxCores;
    public int? maxUses;
    public bool protectedParam;
    public bool requireHeartbeat;
    public int? heartbeatDuration;
    public string? heartbeatCullStrategy;
    public string? heartbeatResurrectionStrategy;
    public string? heartbeatBasis;
    public string? fingerprintUniquenessStrategy;
    public string? fingerprintMatchingStrategy;
    public string? expirationStrategy;
    public string? expirationBasis;
    public string? transferStrategy;
    public string? authenticationStrategy;
    public string? overageStrategy;
    public string? leasingStrategy;
    public Links? links;
    public Data? data;
}