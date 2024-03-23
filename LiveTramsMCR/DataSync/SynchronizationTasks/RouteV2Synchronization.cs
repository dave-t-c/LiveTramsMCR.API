using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class RouteV2Synchronization : ISynchronizationTask<RouteV2>
{
    /// <inheritdoc />
    public async Task SyncData(IMongoClient mongoClient, List<RouteV2> staticData)
    {
        var db = mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        var routesCollection = db.GetCollection<RouteV2>(AppConfiguration.RoutesV2CollectionName);

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

    private static async Task CreateRoutes(IMongoCollection<RouteV2> routesCollection, IEnumerable<RouteV2> routesToCreate)
    {
        await routesCollection.InsertManyAsync(routesToCreate);
    }

    private static async Task DeleteRoutes(IMongoCollection<RouteV2> routesCollection, IEnumerable<RouteV2> routesToDelete)
    {
        foreach (var routeToDelete in routesToDelete)
        {
            await routesCollection.FindOneAndDeleteAsync(stop => stop.Name == routeToDelete.Name);
        }
    }

    private static async Task UpdateExistingRoutes(
        IMongoCollection<RouteV2> routesCollection,
        IEnumerable<RouteV2> routesToUpdate)
    {
        foreach (var route in routesToUpdate)
        {
            await routesCollection.FindOneAndReplaceAsync(existingStop => existingStop.Name == route.Name, route);
        }
    }
}