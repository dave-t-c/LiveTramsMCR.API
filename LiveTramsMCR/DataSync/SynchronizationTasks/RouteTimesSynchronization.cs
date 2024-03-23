using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.RoutePlanner;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class RouteTimesSynchronization : ISynchronizationTask<RouteTimes>
{
    /// <inheritdoc />
    public async Task SyncData(IMongoClient mongoClient, List<RouteTimes> staticData)
    {
        var db = mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        var routeTimesCollection = db.GetCollection<RouteTimes>(AppConfiguration.RouteTimesCollectionName);

        var existingRouteTimes = (await routeTimesCollection.FindAsync(_ => true)).ToList();
        
        var routeTimesToCreate =
            staticData.Where(stop => existingRouteTimes.All(existingStop => existingStop.Route != stop.Route)).ToList();

        var routeTimesToDelete =
            existingRouteTimes.Where(existingStop => staticData.All(stop => existingStop.Route != stop.Route));

        await DeleteRouteTimes(routeTimesCollection, routeTimesToDelete);
        
        await UpdateExistingRouteTimes(routeTimesCollection, staticData);
        
        if (routeTimesToCreate.Any())
        {
            await CreateRouteTimes(routeTimesCollection, routeTimesToCreate);
        }
    }

    private static async Task CreateRouteTimes(IMongoCollection<RouteTimes> routesCollection, IEnumerable<RouteTimes> routeTimesToCreate)
    {
        await routesCollection.InsertManyAsync(routeTimesToCreate);
    }

    private static async Task DeleteRouteTimes(IMongoCollection<RouteTimes> routeTimesCollection, IEnumerable<RouteTimes> routeTimesToDelete)
    {
        foreach (var routeTimeToDelete in routeTimesToDelete)
        {
            await routeTimesCollection.FindOneAndDeleteAsync(routeTimes => routeTimes.Route == routeTimeToDelete.Route);
        }
    }

    private static async Task UpdateExistingRouteTimes(
        IMongoCollection<RouteTimes> routeTimesCollection,
        IEnumerable<RouteTimes> routeTimesToUpdate)
    {
        foreach (var routeTimes in routeTimesToUpdate)
        {
            await routeTimesCollection.FindOneAndReplaceAsync(
                existingRouteTimes => existingRouteTimes.Route == routeTimes.Route, 
                routeTimes);
        }
    }
}