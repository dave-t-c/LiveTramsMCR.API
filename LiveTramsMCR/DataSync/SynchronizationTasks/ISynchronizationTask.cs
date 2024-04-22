using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <summary>
/// Specifies a task to synchronize static data
/// </summary>
public interface ISynchronizationTask<T>
{
    /// <summary>
    /// Executes a task to sync static data
    /// </summary>
    /// <param name="staticData">Latest copy of data to upsert</param>
    public Task SyncData(List<T> staticData);
}