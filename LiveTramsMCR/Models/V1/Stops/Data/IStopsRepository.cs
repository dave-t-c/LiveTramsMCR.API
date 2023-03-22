using System.Collections.Generic;

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
    public Stop GetStop(string searchTerm);

    /// <summary>
    /// Gets all stops
    /// </summary>
    /// <returns>A list of all stops</returns>
    public List<Stop> GetAll();
}