using MongoDB.Driver;

namespace LiveTramsMCR.DataSync.SynchronizationTasks;

/// <summary>
/// Implemented by classes that want to be synced as part of a static data sync task.
/// </summary>
public interface ISynchronizationType<T>
{
    /// <summary>
    /// Return true if data is different to other data
    /// Return false if data is the same
    /// </summary>
    /// <param name="otherData">Date to compare</param>
    public bool CompareSyncData(T otherData);

    /// <summary>
    /// Build a filter definition to identify the entry
    /// in mongoDb
    /// </summary>
    /// <returns></returns>
    public FilterDefinition<T> BuildFilter();
}