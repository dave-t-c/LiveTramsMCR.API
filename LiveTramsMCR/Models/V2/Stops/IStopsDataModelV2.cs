using System.Collections.Generic;

namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
///     Interface that should be implemented
///     by a class that wants to act as a data model
///     for stop information.
/// </summary>
public interface IStopsDataModelV2
{
    /// <summary>
    ///     Returns all stops available in the system.
    /// </summary>
    /// <returns>All available stops as a List</returns>
    public List<StopV2> GetStops();
}