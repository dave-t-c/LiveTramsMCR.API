#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace LiveTramsMCR.Configuration;

public static partial class AppConfiguration
{
    public const string DatabaseName = "livetramsmcr";
    
    public const string StopsCollectionName = "stops";

    public const string StopsV2CollectionName = "stopsV2";

    public const string RoutesCollectionName = "routes";

    public const string RoutesV2CollectionName = "routesV2";

    public const string RouteTimesCollectionName = "route-times";

    public const string MigrationModeVariable = "MIGRATIONMODE";

    public const string MigrationModeCreateValue = "CREATE";

    public const string DynamoDbEnabledKey = "DynamoDbEnabled";

    public const int DefaultReadCapacityUnits = 1;

    public const int DefaultWriteCapacityUnits = 1;
}