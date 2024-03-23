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
    public async Task SyncData(IMongoClient mongoClient, List<Route> staticData)
    {
        var db = mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        var routesCollection = db.GetCollection<Route>(AppConfiguration.RoutesCollectionName);

        var existingRoutes = (await routesCollection.FindAsync(_ => true)).ToList();
        
        var routesToCreate =
            staticData.Where(stop => existingRoutes.All(existingStop => existingStop.Name != stop.Name)).ToList();

        var routesToDelete =
            existingRoutes.Where(existingStop => staticData.All(stop => existingStop.Name != stop.Name));

        await DeleteRoutes(routesCollection, routesToDelete);
        
        await UpdateExistingRoutes(routesCollection, staticData);
        
        if (routesToCreate.Any())
        {
            await CreateRoutes(routesCollection, routesToCreate);
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