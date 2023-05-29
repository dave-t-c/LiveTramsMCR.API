using System.Collections.Generic;

namespace LiveTramsMCR.Models.V2.RoutePlanner;

/// <summary>
/// Data model for processing route related requests
/// </summary>
public interface IRoutesDataModel
{
    /// <summary>
    /// Returns all routes
    /// </summary>
    public List<RouteV2> GetAllRoutes();
}