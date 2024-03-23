using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V2.Stops;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class StopV2Synchronization : ISynchronizationTask<StopV2>
{
    /// <inheritdoc />
    public async Task SyncData(IMongoCollection<StopV2> mongoCollection, List<StopV2> staticData)
    {
        var existingStops = (await mongoCollection.FindAsync(_ => true)).ToList();
        
        var stopsToCreate =
            staticData.Where(stop => existingStops.All(existingStop => existingStop.Tlaref != stop.Tlaref)).ToList();

        var stopsToDelete =
            existingStops.Where(existingStop => staticData.All(stop => existingStop.Tlaref != stop.Tlaref));

        await DeleteStops(mongoCollection, stopsToDelete);
        
        await UpdateExistingStops(mongoCollection, staticData);
        
        if (stopsToCreate.Any())
        {
            await CreateStops(mongoCollection, stopsToCreate);
        }
    }

    private static async Task CreateStops(IMongoCollection<StopV2> stopsCollection, IEnumerable<StopV2> stopsToCreate)
    {
        await stopsCollection.InsertManyAsync(stopsToCreate);
    }

    private static async Task DeleteStops(IMongoCollection<StopV2> stopsCollection, IEnumerable<StopV2> stopsToDelete)
    {
        foreach (var stopToDelete in stopsToDelete)
        {
            await stopsCollection.FindOneAndDeleteAsync(stop => stop.Tlaref == stopToDelete.Tlaref);
        }
    }

    private static async Task UpdateExistingStops(
        IMongoCollection<StopV2> stopsCollection,
        IEnumerable<StopV2> stopsToUpdate)
    {
        foreach (var stop in stopsToUpdate)
        {
            await stopsCollection.FindOneAndReplaceAsync(existingStop => existingStop.Tlaref == stop.Tlaref, stop);
        }
    }
}