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
    public async Task SyncData(IMongoClient mongoClient, IEnumerable<Stop> staticData)
    {
        var db = mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        var stopsCollection = db.GetCollection<Stop>(AppConfiguration.StopsCollectionName);

        var existingStops = (await stopsCollection.FindAsync(_ => true)).ToList();
        
        var stopsToCreate =
            staticData.Where(stop => existingStops.All(existingStop => existingStop.Tlaref != stop.Tlaref)).ToList();

        await CreateStops(stopsCollection, stopsToCreate);
    }

    private async Task CreateStops(IMongoCollection<Stop> stopsCollection, IEnumerable<Stop> stopsToCreate)
    {
        await stopsCollection.InsertManyAsync(stopsToCreate);
    }
}