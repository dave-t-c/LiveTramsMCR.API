using System.Collections.Generic;

namespace LiveTramsMCR.Models.V1.Stops.Data;

/// <summary>
///     Retrieves stop data
/// </summary>
public interface IStopsRepository
{
    /// <summary>
    ///     Gets a stop object for a given stop tlaref
    /// </summary>
    /// <param name="stopTlaref">Stop or tlaref</param>
    /// <returns></returns>
    public Stop GetStop(string stopTlaref);

    /// <summary>
    ///     Gets all stops
    /// </summary>
    /// <returns>A list of all stops</returns>
    public List<Stop> GetAll();
}