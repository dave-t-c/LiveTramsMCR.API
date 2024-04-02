using System;

namespace LiveTramsMCR.Configuration;

public static class FeatureFlags
{
    public static bool DynamoDbEnabled => string.Equals("true",
        Environment.GetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey),
        StringComparison.InvariantCultureIgnoreCase);
}