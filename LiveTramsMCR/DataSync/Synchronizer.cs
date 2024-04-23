using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Common.Data.DynamoDb;
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

        await RunSyncTask<Stop>(
            db,
            request.DynamoDbClient,
            request.DynamoDbContext,
            request.StopsCollectionName,
            request.StopsPath);
        
        await RunSyncTask<RouteTimes>(
            db,
            request.DynamoDbClient,
            request.DynamoDbContext,
            request.RouteTimesCollectionName, 
            request.RouteTimesPath);

        await RunSyncTask<Route>(
            db,
            request.DynamoDbClient,
            request.DynamoDbContext,
            request.RoutesCollectionName,
            request.RoutesPath);
        
        await RunSyncTask<StopV2>(
            db,
            request.DynamoDbClient,
            request.DynamoDbContext,
            request.StopsV2CollectionName, 
            request.StopsV2Path);
        
        await RunSyncTask<RouteV2>(
            db,
            request.DynamoDbClient,
            request.DynamoDbContext,
            request.RoutesV2CollectionName, 
            request.RoutesV2Path);
    }

    private static async Task RunSyncTask<T>(
        IMongoDatabase db,
        IAmazonDynamoDB dynamoDbClient,
        IDynamoDBContext dynamoDbContext,
        string collectionName,
        string configPath)
    where T: ISynchronizationType<T>, IDynamoDbTable
    {
        var mongoCollection = db.GetCollection<T>(collectionName);
        var staticDataPath = Path.Combine(Environment.CurrentDirectory, configPath);
        var importedStaticData = FileHelper.ImportFromJsonFile<List<T>>(staticDataPath);

        var syncTask = new SynchronizationTask<T>(mongoCollection, dynamoDbClient, dynamoDbContext);
        await syncTask.SyncData(importedStaticData);
    }
}