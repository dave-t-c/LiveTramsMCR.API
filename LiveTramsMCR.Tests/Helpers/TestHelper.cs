using System;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
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
        var db = mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        var stopsMongoCollection = db.GetCollection<Stop>(AppConfiguration.StopsCollectionName);
        var stopsV2MongoCollection = db.GetCollection<StopV2>(AppConfiguration.StopsV2CollectionName);
        var routesMongoCollection = db.GetCollection<Route>(AppConfiguration.RoutesCollectionName);
        var routeTimesMongoCollection = db.GetCollection<RouteTimes>(AppConfiguration.RouteTimesCollectionName);
        IStopsRepository stopsRepository = new StopsRepository(stopsMongoCollection);
        IRouteRepository routeRepository = new RouteRepository(routesMongoCollection, routeTimesMongoCollection);
        IStopsRepositoryV2 stopsRepositoryV2 = new StopsRepositoryV2(stopsV2MongoCollection);

        services.AddSingleton(mongoClient);
        services.AddSingleton(stopsRepository);
        services.AddSingleton(stopsRepositoryV2);
        services.AddSingleton(routeRepository);
        
        return services.BuildServiceProvider();
    }

    public static T GetService<T>()
    where T : notnull
    {
        _provider ??= Provider();
        return _provider.GetRequiredService<T>();
    }
}