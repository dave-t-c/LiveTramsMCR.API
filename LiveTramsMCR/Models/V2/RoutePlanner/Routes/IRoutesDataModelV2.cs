using System.Collections.Generic;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Routes;

/// <summary>
/// Data model for processing route related requests
/// </summary>
public interface IRoutesDataModelV2
{
    /// <summary>
    /// Returns all routes
    /// </summary>
    public List<RouteV2> GetAllRoutes();
}