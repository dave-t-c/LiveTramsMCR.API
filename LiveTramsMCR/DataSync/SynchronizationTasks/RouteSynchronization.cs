using System.Collections.Generic;
using System.Threading.Tasks;
using LiveTramsMCR.Models.V1.RoutePlanner;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class RouteSynchronization : ISynchronizationTask<Route>
{
    /// <inheritdoc />
    public async Task SyncData(IMongoCollection<Route> mongoCollection, List<Route> staticData)
    {
        var task = new SynchronizationTask<Route>();
        await task.SyncData(mongoCollection, staticData);
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