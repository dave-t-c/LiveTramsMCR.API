using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V2.Stops.Data;

/// <inheritdoc />
public class StopsRepositoryV2 : IStopsRepositoryV2
{
    private readonly IMongoCollection<StopV2> _stopsCollection;

    /// <summary>
    ///     Create a new stops repository using a stops collection
    /// </summary>
    /// <param name="stopsCollection"></param>
    public StopsRepositoryV2(IMongoCollection<StopV2> stopsCollection)
    {
        _stopsCollection = stopsCollection;
    }

    /// <inheritdoc />
    public StopV2 GetStop(string searchTerm)
    {
        return _stopsCollection.FindAsync(stop =>
            stop.StopName.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
            || stop.Tlaref.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
        ).Result.FirstOrDefault();
    }

    /// <inheritdoc />
    public List<StopV2> GetAll()
    {
        return _stopsCollection.FindAsync(_ => true).Result.ToList();
    }
}