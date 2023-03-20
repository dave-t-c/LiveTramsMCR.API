using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveTramsMCR.Models.V1.Stops.Data;

/// <summary>
/// Retrieves stop data
/// </summary>
public interface IStopsRepository
{
    /// <summary>
    /// Gets a stop object for a given stop name
    /// or tlaref
    /// </summary>
    /// <param name="searchTerm">Stop name or tlaref</param>
    /// <returns></returns>
    public Task<Stop> GetStop(string searchTerm);

    /// <summary>
    /// Gets the associated IDs for a given stop by stop name
    /// or stop tlaref
    /// </summary>
    /// <param name="searchTerm">Stop name or tlaref</param>
    /// <returns></returns>
    public Task<List<int>> GetStopIds(string searchTerm);

    /// <summary>
    /// Gets all stops
    /// </summary>
    /// <returns>A list of all stops</returns>
    public Task<List<Stop>> GetAll();
}