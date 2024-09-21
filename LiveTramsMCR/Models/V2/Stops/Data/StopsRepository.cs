using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Configuration;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V2.Stops.Data;

/// <inheritdoc />
public class StopsRepositoryV2 : IStopsRepositoryV2
{
    private readonly IMongoCollection<StopV2> _stopsCollection;
    private readonly IDynamoDBContext _context;

    /// <summary>
    ///     Create a new stops repository using a stops collection
    /// </summary>
    public StopsRepositoryV2(IMongoCollection<StopV2> stopsCollection, IDynamoDBContext context)
    {
        _stopsCollection = stopsCollection;
        _context = context;
    }

    /// <inheritdoc />
    public StopV2 GetStop(string stopTlaref)
    {
        StopV2 result;
        if (FeatureFlags.DynamoDbEnabled)
        {
            result = _context.LoadAsync<StopV2>(stopTlaref).Result;
        }
        else
        {
            result = _stopsCollection.FindAsync(stop =>
                stop.Tlaref.Equals(stopTlaref, StringComparison.OrdinalIgnoreCase)
            ).Result.FirstOrDefault();
        }

        return result;
    }

    /// <inheritdoc />
    public List<StopV2> GetAll()
    {
        List<StopV2> stops;
        if (FeatureFlags.DynamoDbEnabled)
        {
            stops = _context.ScanAsync<StopV2>(default).GetRemainingAsync().Result.ToList();
        }
        else
        {
            stops = _stopsCollection.FindAsync(_ => true).Result.ToList();
        }

        return stops.OrderBy(stop => stop.StopName).ToList();
    }
}