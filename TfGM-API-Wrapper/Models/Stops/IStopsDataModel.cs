using System.Collections.Generic;

namespace TfGM_API_Wrapper.Models.Stops;

/// <summary>
/// Interface that should be implemented
/// by a class that wants to act as a data model
/// for stop information.
/// </summary>
public interface IStopsDataModel
{
    /// <summary>
    /// Returns all stops available in the system.
    /// </summary>
    /// <returns>All available stops as a List</returns>
    public List<Stop> GetStops();
}