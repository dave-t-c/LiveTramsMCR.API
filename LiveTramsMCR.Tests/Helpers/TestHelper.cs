using System;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
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
        IStopsRepository stopsRepository = new StopsRepository(stopsMongoCollection);

        services.AddSingleton(mongoClient);
        services.AddSingleton(stopsRepository);
        
        return services.BuildServiceProvider();
    }

    public static T GetService<T>()
    where T : notnull
    {
        _provider ??= Provider();
        return _provider.GetRequiredService<T>();
    }
}