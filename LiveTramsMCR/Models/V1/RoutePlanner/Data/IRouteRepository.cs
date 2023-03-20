using System.Threading.Tasks;

namespace LiveTramsMCR.Models.V1.RoutePlanner.Data;

/// <summary>
/// Retrieves route data
/// </summary>
public interface IRouteRepository
{
    /// <summary>
    /// Gets the route for a given route name
    /// </summary>
    /// <param name="routeName">Name of route</param>
    /// <returns></returns>
    public Task<Route> GetRouteByNameAsync(string routeName);

    /// <summary>
    /// Gets the route times for a given route
    /// </summary>
    /// <param name="routeName">Name of the route</param>
    /// <returns></returns>
    public Task<RouteTimes> GetRouteTimesByNameAsync(string routeName);
}