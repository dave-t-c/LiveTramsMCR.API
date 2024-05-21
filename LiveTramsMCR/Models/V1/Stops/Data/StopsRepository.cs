using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Configuration;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V1.Stops.Data;

/// <inheritdoc />
public class StopsRepository : IStopsRepository
{
    private readonly IMongoCollection<Stop> _stopsCollection;
    private readonly IDynamoDBContext _context;
    
    /// <summary>
    ///     Create a new stops repository using a stops collection
    /// </summary>
    public StopsRepository(IMongoCollection<Stop> stopsCollection, IDynamoDBContext context)
    {
        _stopsCollection = stopsCollection;
        _context = context;
    }

    /// <inheritdoc />
    public Stop GetStop(string stopTlaref)
    {
        Stop result;
        if (FeatureFlags.DynamoDbEnabled)
        {
            result = _context.LoadAsync<Stop>(stopTlaref).Result;
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
    public List<Stop> GetAll()
    {
        List<Stop> result;
        if (FeatureFlags.DynamoDbEnabled)
        {
            
            result = _context.ScanAsync<Stop>(default).GetRemainingAsync().Result
                .OrderBy(stop => stop.StopName)
                .ToList();
        }
        else
        {
            result = _stopsCollection.FindAsync(_ => true).Result.ToList();
        }

        return result;
    }
}