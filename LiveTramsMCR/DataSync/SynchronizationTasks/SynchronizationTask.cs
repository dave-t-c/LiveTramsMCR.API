using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

public class SynchronizationTask<T>: ISynchronizationTask<T> 
where T: ISynchronizationType<T>
{
    public async Task SyncData(IMongoCollection<T> mongoCollection, List<T> staticData)
    {
        var existingDataValues = (await mongoCollection.FindAsync(_ => true)).ToList();
        
        var dataToCreate  =
            staticData.Where(newData => existingDataValues.TrueForAll(existingData => existingData.CompareSyncData(newData))).ToList();

        var dataToDelete =
            existingDataValues.Where(existingData =>
                staticData.TrueForAll(newData => existingData.CompareSyncData(newData)));

        await DeleteData(mongoCollection, dataToDelete);

        await UpdateExistingData(mongoCollection, staticData);

        if (dataToCreate.Any())
        {
            await CreateData(mongoCollection, dataToCreate);
        }

    }
    
    private static async Task CreateData(IMongoCollection<T> mongoCollection, IEnumerable<T> dataToCreate)
    {
        await mongoCollection.InsertManyAsync(dataToCreate);
    }

    private static async Task DeleteData(IMongoCollection<T> mongoCollection, IEnumerable<T> dataToDelete)
    {
        foreach (var data in dataToDelete)
        {
            var filter = data.BuildFilter();
            await mongoCollection.FindOneAndDeleteAsync(filter);
        }
    }

    private static async Task UpdateExistingData(
        IMongoCollection<T> mongoCollection,
        IEnumerable<T> dataToUpdate)
    {
        foreach (var data in dataToUpdate)
        {
            var filter = data.BuildFilter();
            await mongoCollection.FindOneAndReplaceAsync(filter, data);
        }
    }
}