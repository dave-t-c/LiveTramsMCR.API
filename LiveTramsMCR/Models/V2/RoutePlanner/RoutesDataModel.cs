using System.Collections.Generic;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;

namespace LiveTramsMCR.Models.V2.RoutePlanner;

/// <inheritdoc/>
public class RoutesDataModel : IRoutesDataModel
{
    private readonly IRouteRepositoryV2 _routeRepositoryV2;

    /// <summary>
    /// Creates a Routes data model which will query the routes repository provided.
    /// </summary>
    public RoutesDataModel(IRouteRepositoryV2 routeRepositoryV2)
    {
        _routeRepositoryV2 = routeRepositoryV2;
    }
    
    /// <inheritdoc/>
    public List<RouteV2> GetAllRoutes()
    {
        return _routeRepositoryV2.GetRoutes();
    }
}