using System;

namespace LiveTramsMCR.Configuration;

/// <summary>
/// Specifies if certain features should be enabled or disabled
/// </summary>
public static class FeatureFlags
{
    /// <summary>
    /// Should DynamoDb be used instead of cosmos db.
    /// </summary>
    public static bool DynamoDbEnabled => string.Equals("true",
        Environment.GetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey),
        StringComparison.InvariantCultureIgnoreCase);
}