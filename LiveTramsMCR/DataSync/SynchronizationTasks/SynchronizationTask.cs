using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class SynchronizationTask<T>: ISynchronizationTask<T> 
where T: ISynchronizationType<T>
{
    private IMongoCollection<T> _mongoCollection;

    /// <summary>
    /// Creates a new sync task for a given Synchronisation type.
    /// </summary>
    /// <param name="mongoCollection">Mongo collection to use to sync data</param>
    public SynchronizationTask(IMongoCollection<T> mongoCollection)
    {
        this._mongoCollection = mongoCollection;
    }
    
    public async Task SyncData(List<T> staticData)
    {
        var existingDataValues = (await this._mongoCollection.FindAsync(_ => true)).ToList();
        
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
    
    private async Task CreateData(IEnumerable<T> dataToCreate)
    {
        await this._mongoCollection.InsertManyAsync(dataToCreate);
    }

    private async Task DeleteData(IEnumerable<T> dataToDelete)
    {
        foreach (var data in dataToDelete)
        {
            var filter = data.BuildFilter();
            await this._mongoCollection.FindOneAndDeleteAsync(filter);
        }
    }

    private async Task UpdateExistingData(IEnumerable<T> dataToUpdate)
    {
        foreach (var data in dataToUpdate)
        {
            var filter = data.BuildFilter();
            await this._mongoCollection.FindOneAndReplaceAsync(filter, data);
        }
    }
}