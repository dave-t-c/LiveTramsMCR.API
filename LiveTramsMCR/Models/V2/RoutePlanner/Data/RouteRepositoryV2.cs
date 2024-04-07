using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Data;

/// <inheritdoc />
public class RouteRepositoryV2 : IRouteRepositoryV2
{
    private readonly IMongoCollection<RouteV2> _routesCollection;
    private readonly IDynamoDBContext _context;
    private readonly IStopsRepositoryV2 _stopsRepositoryV2;

    /// <summary>
    ///     Create a new routes repository using a routes collection.
    ///     Builds the route details using the stops collection provided.
    /// </summary>
    public RouteRepositoryV2(IMongoCollection<RouteV2> routesCollection, IDynamoDBContext context, IStopsRepositoryV2 stopsRepositoryV2)
    {
        _routesCollection = routesCollection;
        _context = context;
        _stopsRepositoryV2 = stopsRepositoryV2;
    }

    /// <inheritdoc />
    public List<RouteV2> GetRoutes()
    {
        List<RouteV2> routes;
        if (FeatureFlags.DynamoDbEnabled)
        {
            routes = _context.ScanAsync<RouteV2>(default).GetRemainingAsync().Result.ToList();
        }
        else
        {
            routes = _routesCollection.FindAsync(_ => true).Result.ToList();

        }
        
        var stops = _stopsRepositoryV2.GetAll();
        PopulateRoutes(routes, stops);
        return routes;
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