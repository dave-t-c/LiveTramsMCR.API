using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.Stops;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class StopSynchronization : ISynchronizationTask<Stop>
{
    /// <inheritdoc />
    public async Task SyncData(IMongoCollection<Stop> mongoCollection, List<Stop> staticData)
    {
        var existingStops = (await mongoCollection.FindAsync(_ => true)).ToList();
        
        var stopsToCreate =
            staticData.Where(stop => existingStops.TrueForAll(existingStop => existingStop.Tlaref != stop.Tlaref)).ToList();

        var stopsToDelete =
            existingStops.Where(existingStop => staticData.TrueForAll(stop => existingStop.Tlaref != stop.Tlaref));

        await DeleteStops(mongoCollection, stopsToDelete);
        
        await UpdateExistingStops(mongoCollection, staticData);
        
        if (stopsToCreate.Any())
        {
            await CreateStops(mongoCollection, stopsToCreate);
        }
    }

    private static async Task CreateStops(IMongoCollection<Stop> stopsCollection, IEnumerable<Stop> stopsToCreate)
    {
        await stopsCollection.InsertManyAsync(stopsToCreate);
    }

    private static async Task DeleteStops(IMongoCollection<Stop> stopsCollection, IEnumerable<Stop> stopsToDelete)
    {
        foreach (var stopToDelete in stopsToDelete)
        {
            await stopsCollection.FindOneAndDeleteAsync(stop => stop.Tlaref == stopToDelete.Tlaref);
        }
    }

    private static async Task UpdateExistingStops(
        IMongoCollection<Stop> stopsCollection,
        IEnumerable<Stop> stopsToUpdate)
    {
        foreach (var stop in stopsToUpdate)
        {
            await stopsCollection.FindOneAndReplaceAsync(existingStop => existingStop.Tlaref == stop.Tlaref, stop);
        }
    }
}