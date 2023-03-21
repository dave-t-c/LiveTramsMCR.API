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
    public Task<Stop> GetStop(string searchTerm)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<List<int>> GetStopIds(string searchTerm)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public List<Stop> GetAll()
    {
        return _stopsCollection.FindAsync(_ => true).Result.ToList();
    }
}