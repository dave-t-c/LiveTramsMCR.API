using System.Collections.Generic;
using System.Linq;
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
    
    public RouteTimes? GetRouteTimesByNameAsync(string routeName)
    {
        return _routeTimes.FirstOrDefault(route => route.Route == routeName);
    }

    public List<Route> GetAllRoutesAsync()
    {
        return _routes;
    }

    public void UpdateRoutes(List<Route> routes)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateRoute(Route route)
    {
        throw new System.NotImplementedException();
    }
}