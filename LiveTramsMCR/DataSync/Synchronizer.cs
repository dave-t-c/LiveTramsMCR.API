using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LiveTramsMCR.DataSync.Helpers;
using LiveTramsMCR.DataSync.SynchronizationTasks;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync;

/// <summary>
/// Synchronizes static json files with DB. 
/// </summary>
public class Synchronizer
{
    /// <summary>
    /// Run synchronization of static data files
    /// </summary>
    /// <param name="request">Request params for sync config.</param>
    public async Task SynchronizeStaticData(SynchronizationRequest request)
    {
        var db = request.MongoClient.GetDatabase(request.TargetDbName);

        await RunSyncTask<Stop>(db, request.StopsCollectionName,
            request.StopsPath);
        
        await RunSyncTask<RouteTimes, RouteTimesSynchronization>(db, request.RouteTimesCollectionName, 
            request.RouteTimesPath);

        await RunSyncTask<Route>(db, request.RoutesCollectionName,
            request.RoutesPath);
        
        await RunSyncTask<StopV2, StopV2Synchronization>(db, request.StopsV2CollectionName, 
            request.StopsV2Path);
        
        await RunSyncTask<RouteV2, RouteV2Synchronization>(db, request.RoutesV2CollectionName, 
            request.RoutesV2Path);
    }

    private static async Task RunSyncTask<T, TU>(
        IMongoDatabase db,
        string collectionName,
        string configPath)
    where TU : ISynchronizationTask<T>
    {
        var mongoCollection = db.GetCollection<T>(collectionName);
        var staticDataPath = Path.Combine(Environment.CurrentDirectory, configPath);
        var importedStaticData = FileHelper.ImportFromJsonFile<List<T>>(staticDataPath);

        var syncTask = (ISynchronizationTask<T>)Activator.CreateInstance(typeof(TU));
        await syncTask?.SyncData(mongoCollection, importedStaticData)!;
    }

    private static async Task RunSyncTask<T>(
        IMongoDatabase db,
        string collectionName,
        string configPath)
    where T: ISynchronizationType<T>
    {
        var mongoCollection = db.GetCollection<T>(collectionName);
        var staticDataPath = Path.Combine(Environment.CurrentDirectory, configPath);
        var importedStaticData = FileHelper.ImportFromJsonFile<List<T>>(staticDataPath);

        var syncTask = new SynchronizationTask<T>();
        await syncTask.SyncData(mongoCollection, importedStaticData);
    }
}