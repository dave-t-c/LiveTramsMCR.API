using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V2.Stops;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class RouteSynchronization : ISynchronizationTask<Route>
{
    /// <inheritdoc />
    public async Task SyncData(IMongoCollection<Route> mongoCollection, List<Route> staticData)
    {
        var existingRoutes = (await mongoCollection.FindAsync(_ => true)).ToList();
        
        var routesToCreate =
            staticData.Where(stop => existingRoutes.All(existingStop => existingStop.Name != stop.Name)).ToList();

        var routesToDelete =
            existingRoutes.Where(existingStop => staticData.All(stop => existingStop.Name != stop.Name));

        await DeleteRoutes(mongoCollection, routesToDelete);
        
        await UpdateExistingRoutes(mongoCollection, staticData);
        
        if (routesToCreate.Any())
        {
            await CreateRoutes(mongoCollection, routesToCreate);
        }
    }

    private static async Task CreateRoutes(IMongoCollection<Route> routesCollection, IEnumerable<Route> routesToCreate)
    {
        await routesCollection.InsertManyAsync(routesToCreate);
    }

    private static async Task DeleteRoutes(IMongoCollection<Route> routesCollection, IEnumerable<Route> routesToDelete)
    {
        foreach (var routeToDelete in routesToDelete)
        {
            await routesCollection.FindOneAndDeleteAsync(route => route.Name == routeToDelete.Name);
        }
    }

    private static async Task UpdateExistingRoutes(
        IMongoCollection<Route> routesCollection,
        IEnumerable<Route> routesToUpdate)
    {
        foreach (var route in routesToUpdate)
        {
            await routesCollection.FindOneAndReplaceAsync(existingRoute => existingRoute.Name == route.Name, route);
        }
    }
}