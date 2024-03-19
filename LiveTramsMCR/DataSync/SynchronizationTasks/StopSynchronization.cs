using System.Collections.Generic;
using System.Threading.Tasks;
using LiveTramsMCR.Models.V1.Stops;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class StopSynchronization : ISynchronizationTask<Stop>
{
    /// <inheritdoc />
    public Task SyncData(IMongoClient mongoClient, IEnumerable<Stop> staticData)
    {
        throw new System.NotImplementedException();
    }
}