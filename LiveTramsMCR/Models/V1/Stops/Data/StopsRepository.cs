using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V1.Stops.Data;

/// <inheritdoc />
public class StopsRepository : IStopsRepository
{
    private readonly IMongoCollection<Stop> _stopsCollection;
    
    /// <summary>
    /// Create a new stops repository using a stops collection
    /// </summary>
    /// <param name="stopsCollection"></param>
    public StopsRepository(IMongoCollection<Stop> stopsCollection)
    {
        _stopsCollection = stopsCollection;
    }
    
    /// <inheritdoc />
    public Stop GetStop(string searchTerm)
    {
        return _stopsCollection.FindAsync(stop =>
                stop.StopName == searchTerm || stop.Tlaref == searchTerm).Result.FirstOrDefault();
    }

    /// <inheritdoc />
    public List<Stop> GetAll()
    {
        return _stopsCollection.FindAsync(_ => true).Result.ToList();
    }
}