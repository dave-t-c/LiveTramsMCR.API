using System.Collections.Generic;
using LiveTramsMCR.Models.V2.RoutePlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;

namespace LiveTramsMCR.Tests.Mocks;

public class MockRouteRepositoryV2 : IRouteRepositoryV2
{
    private readonly List<RouteV2> _importedRoutes;

    public MockRouteRepositoryV2(List<RouteV2> importedRoutes)
    {
        _importedRoutes = importedRoutes;
    }
    
    public List<RouteV2> GetRoutes()
    {
        return _importedRoutes;
    }
}