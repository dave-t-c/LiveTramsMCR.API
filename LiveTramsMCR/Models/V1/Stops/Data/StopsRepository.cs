using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveTramsMCR.Models.V1.Stops.Data;

/// <inheritdoc />
public class StopsRepository : IStopsRepository
{
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
    public Task<List<Stop>> GetAll()
    {
        throw new System.NotImplementedException();
    }
}