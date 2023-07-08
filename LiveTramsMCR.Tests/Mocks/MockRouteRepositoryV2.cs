using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;

namespace LiveTramsMCR.Tests.Mocks;

public class MockRouteRepositoryV2 : IRouteRepositoryV2
{
    private readonly List<RouteV2> _importedRoutes;
    private readonly IStopsRepositoryV2 _stopsRepositoryV2;

    public MockRouteRepositoryV2(List<RouteV2> importedRoutes, IStopsRepositoryV2 stopsRepositoryV2)
    {
        _importedRoutes = importedRoutes;
        _stopsRepositoryV2 = stopsRepositoryV2;
    }

    public List<RouteV2> GetRoutes()
    {
        var stops = _stopsRepositoryV2.GetAll();
        PopulateRoutes(_importedRoutes, stops);
        return _importedRoutes;
    }

    private static void PopulateRoutes(List<RouteV2> routes, List<StopV2> stops)
    {
        routes.ForEach(route => PopulateRoute(route, stops));
    }

    private static void PopulateRoute(RouteV2 route, List<StopV2> stops)
    {
        route.StopsDetail = new List<StopV2>();
        foreach (var stopKey in route.Stops)
        {
            var stop = stops.First(s => s.Tlaref == stopKey.Tlaref);
            route.StopsDetail?.Add(stop);
        }
    }
}