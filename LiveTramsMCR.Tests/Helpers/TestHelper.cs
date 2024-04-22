using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Util;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LiveTramsMCR.Tests.Helpers;

public static class TestHelper
{
    private static IServiceProvider? _provider;
    private static IServiceProvider Provider()
    {
        var services = new ServiceCollection();
        var mongoClient = new MongoClient(TestAppConfiguration.TestDbConnectionString);
        
        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "foo");
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "bar");
        var dynamoDbConfig = new AmazonDynamoDBConfig
        {
            MaxErrorRetry = 1,
            RetryMode = RequestRetryMode.Standard,
            LogMetrics = true,
            ServiceURL = "http://localhost:8000",
            Timeout = TimeSpan.FromSeconds(5)
        };
        var awsCredentials = new BasicAWSCredentials("foo", "bar"); 

        IAmazonDynamoDB dynamoDbClient = new AmazonDynamoDBClient(awsCredentials, dynamoDbConfig);
        IDynamoDBContext dynamoDbContext = new DynamoDBContext(dynamoDbClient);
        
        var db = mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        var stopsMongoCollection = db.GetCollection<Stop>(AppConfiguration.StopsCollectionName);
        var stopsV2MongoCollection = db.GetCollection<StopV2>(AppConfiguration.StopsV2CollectionName);
        var routesMongoCollection = db.GetCollection<Route>(AppConfiguration.RoutesCollectionName);
        var routesV2MongoCollection = db.GetCollection<RouteV2>(AppConfiguration.RoutesV2CollectionName);
        var routeTimesMongoCollection = db.GetCollection<RouteTimes>(AppConfiguration.RouteTimesCollectionName);
        IStopsRepository stopsRepository = new StopsRepository(stopsMongoCollection, dynamoDbContext);
        IRouteRepository routeRepository = new RouteRepository(routesMongoCollection, routeTimesMongoCollection, dynamoDbContext);
        IStopsRepositoryV2 stopsRepositoryV2 = new StopsRepositoryV2(stopsV2MongoCollection, dynamoDbContext);
        IRouteRepositoryV2 routeRepositoryV2 = new RouteRepositoryV2(routesV2MongoCollection, dynamoDbContext, stopsRepositoryV2);

        services.AddSingleton(dynamoDbContext);
        services.AddSingleton(dynamoDbClient);
        services.AddSingleton(mongoClient);
        services.AddSingleton(stopsRepository);
        services.AddSingleton(stopsRepositoryV2);
        services.AddSingleton(routeRepository);
        services.AddSingleton(routeRepositoryV2);
        
        return services.BuildServiceProvider();
    }

    public static T GetService<T>()
    where T : notnull
    {
        _provider ??= Provider();
        return _provider.GetRequiredService<T>();
    }
}