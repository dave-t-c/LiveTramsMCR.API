using System.Collections.Generic;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Data;

/// <summary>
/// Retrieves Route Data
/// </summary>
public interface IRouteRepositoryV2
{
    /// <summary>
    /// Returns all routes
    /// </summary>
    public List<RouteV2> GetRoutes();
}