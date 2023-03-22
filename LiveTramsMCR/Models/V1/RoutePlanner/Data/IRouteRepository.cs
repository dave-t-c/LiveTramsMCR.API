#nullable enable
using System.Collections.Generic;

namespace LiveTramsMCR.Models.V1.RoutePlanner.Data;

/// <summary>
/// Retrieves route data
/// </summary>
public interface IRouteRepository
{
    /// <summary>
    /// Gets the route times for a given route
    /// </summary>
    /// <param name="routeName">Name of the route</param>
    /// <returns></returns>
    public RouteTimes? GetRouteTimesByNameAsync(string routeName);

    /// <summary>
    /// Gets all routes
    /// </summary>
    /// <returns>List of routes</returns>
    public List<Route> GetAllRoutesAsync();
}