using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <summary>
/// Specifies a task to synchronize static data
/// </summary>
public interface ISynchronizationTask<in T>
{
    /// <summary>
    /// Executes a task to sync static data
    /// </summary>
    /// <param name="mongoClient">Mongo client to use to sync data</param>
    /// <param name="staticData">Latest copy of data to upsert</param>
    public Task SyncData(IMongoClient mongoClient, IEnumerable<T> staticData);
}