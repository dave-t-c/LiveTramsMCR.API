namespace LiveTramsMCR.Tests.Configuration;

internal static class TestAppConfiguration
{
    internal static double RouteCoordinateTolerance => 0.000001;

    internal static string TestDbConnectionString => "mongodb://foo:bar@mongo:27017";

    internal static string LocalDynamoDbServiceUrl => "http://localstack:4566";
}