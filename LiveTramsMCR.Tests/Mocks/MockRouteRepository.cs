using System.Collections.Generic;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;

namespace LiveTramsMCR.Tests.Mocks;

public class MockRouteRepository : IRouteRepository
{
    private readonly List<Route> _routes;
    private readonly List<RouteTimes> _routeTimes;

    public MockRouteRepository(List<Route> routes, List<RouteTimes> routeTimes)
    {
        _routes = routes;
        _routeTimes = routeTimes;
    }
    
    public RouteTimes GetRouteTimesByNameAsync(string routeName)
    {
        throw new System.NotImplementedException();
    }

    public List<Route> GetAllRoutesAsync()
    {
        throw new System.NotImplementedException();
    }
}