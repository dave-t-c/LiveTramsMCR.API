using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Common.Data.DynamoDb;
using LiveTramsMCR.Configuration;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class SynchronizationTask<T>: ISynchronizationTask<T> 
where T: ISynchronizationType<T>, IDynamoDbTable
{
    private readonly IMongoCollection<T> _mongoCollection;
    private readonly IAmazonDynamoDB _dynamoDbClient;
    private readonly IDynamoDBContext _dynamoDbContext;

    /// <summary>
    /// Creates a new sync task for a given Synchronisation type.
    /// </summary>
    /// <param name="mongoCollection">Mongo collection to use to sync data</param>
    /// <param name="dynamoDbClient">DynamoDb client for retrieving and updating data</param>
    /// <param name="dynamoDbContext">Dynamodb context for modifying data</param>
    public SynchronizationTask(
        IMongoCollection<T> mongoCollection,
        IAmazonDynamoDB dynamoDbClient,
        IDynamoDBContext dynamoDbContext)
    {
        this._mongoCollection = mongoCollection;
        this._dynamoDbClient = dynamoDbClient;
        this._dynamoDbContext = dynamoDbContext;
    }
    
    /// <inheritdoc />
    public async Task SyncData(List<T> staticData)
    {
        var existingDataValues = await RetrieveExistingDataAsync();
        
        var dataToCreate  =
            staticData.Where(newData => existingDataValues.TrueForAll(existingData => existingData.CompareSyncData(newData))).ToList();

        var dataToDelete =
            existingDataValues.Where(existingData =>
                staticData.TrueForAll(newData => existingData.CompareSyncData(newData)));

        await DeleteData(dataToDelete);

        await UpdateExistingData(staticData);

        if (dataToCreate.Any())
        {
            await CreateData(dataToCreate);
        }

    }

    private async Task<List<T>> RetrieveExistingDataAsync()
    {
        if (FeatureFlags.DynamoDbEnabled)
        {
            var result = await DynamoDbHelper.RetrieveExistingRecords<T>(
                _dynamoDbContext,
                _dynamoDbClient);
            return result;
        }
        else
        {
            var result = await this._mongoCollection.FindAsync(_ => true);
            return result.ToList();
        }
    }
    
    private async Task CreateData(IEnumerable<T> dataToCreate)
    {
        if (FeatureFlags.DynamoDbEnabled)
        {
            await DynamoDbHelper.CreateRecords(
                _dynamoDbContext,
                _dynamoDbClient,
                dataToCreate);
        }
        else
        {
            await this._mongoCollection.InsertManyAsync(dataToCreate);
        }
    }

    private async Task DeleteData(IEnumerable<T> dataToDelete)
    {
        if (FeatureFlags.DynamoDbEnabled)
        {
            var dataToDeleteList = dataToDelete.ToList();
            if (dataToDeleteList.Any())
            {
                var batchDelete = this._dynamoDbContext.CreateBatchWrite<T>();
                batchDelete.AddDeleteItems(dataToDeleteList);
                await batchDelete.ExecuteAsync();
            }
        }
        else
        {
            foreach (var data in dataToDelete)
            {
                var filter = data.BuildFilter();
                await this._mongoCollection.FindOneAndDeleteAsync(filter);
            }
        }
    }

    private async Task UpdateExistingData(IEnumerable<T> dataToUpdate)
    {
        if (FeatureFlags.DynamoDbEnabled)
        {
            var tableExists = await DynamoDbHelper.TableExists(typeof(T), _dynamoDbClient);
            if (!tableExists)
                return;
            
            foreach (var data in dataToUpdate)
            {
                await _dynamoDbContext.SaveAsync(data);
            }
        }
        else
        {
            foreach (var data in dataToUpdate)
            {
                var filter = data.BuildFilter();
                await this._mongoCollection.FindOneAndReplaceAsync(filter, data);
            }
        }
    }
}