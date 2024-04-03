using System.Collections.Generic;

namespace LiveTramsMCR.Models.V2.Stops.Data;

/// <summary>
///     Retrieves stop data
/// </summary>
public interface IStopsRepositoryV2
{
    /// <summary>
    ///     Gets a stop object for a given stop tlaref
    /// </summary>
    /// <param name="searchTerm">Stop tlaref</param>
    /// <returns></returns>
    public StopV2 GetStop(string searchTerm);

    /// <summary>
    ///     Gets all stops
    /// </summary>
    /// <returns>A list of all stops</returns>
    public List<StopV2> GetAll();
}