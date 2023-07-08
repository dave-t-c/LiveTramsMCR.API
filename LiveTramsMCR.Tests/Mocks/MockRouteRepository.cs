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

    public List<Route> GetAllRoutes()
    {
        return _routes;
    }

    public void UpdateRoutes(List<Route> routes)
    {
        foreach (var route in routes.ToList())
        {
            UpdateRoute(route);
        }
    }

    public void UpdateRoute(Route route)
    {
        var existing = _routes.Find(s => s.Name == route.Name);
        var existingIndex = _routes.IndexOf(existing!);
        _routes[existingIndex] = route;
    }
}