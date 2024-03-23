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
    public async Task SyncData(IMongoCollection<RouteTimes> mongoCollection, List<RouteTimes> staticData)
    {
        var existingRouteTimes = (await mongoCollection.FindAsync(_ => true)).ToList();
        
        var routeTimesToCreate =
            staticData.Where(stop => existingRouteTimes.All(existingStop => existingStop.Route != stop.Route)).ToList();

        var routeTimesToDelete =
            existingRouteTimes.Where(existingStop => staticData.All(stop => existingStop.Route != stop.Route));

        await DeleteRouteTimes(mongoCollection, routeTimesToDelete);
        
        await UpdateExistingRouteTimes(mongoCollection, staticData);
        
        if (routeTimesToCreate.Any())
        {
            await CreateRouteTimes(mongoCollection, routeTimesToCreate);
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