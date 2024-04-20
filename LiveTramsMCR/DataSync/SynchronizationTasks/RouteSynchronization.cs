using System.Collections.Generic;
using System.Threading.Tasks;
using LiveTramsMCR.Models.V1.RoutePlanner;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <inheritdoc />
public class RouteSynchronization : ISynchronizationTask<Route>
{
    /// <inheritdoc />
    public async Task SyncData(IMongoCollection<Route> mongoCollection, List<Route> staticData)
    {
        var task = new SynchronizationTask<Route>();
        await task.SyncData(mongoCollection, staticData);
    }
}