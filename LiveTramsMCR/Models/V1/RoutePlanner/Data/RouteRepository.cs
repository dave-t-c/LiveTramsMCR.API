using System.Collections.Generic;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V1.RoutePlanner.Data;

/// <inheritdoc />
public class RouteRepository : IRouteRepository
{
    private readonly IMongoCollection<Route> _routeCollection;

    private readonly IMongoCollection<RouteTimes> _routeTimesCollection;
    
    /// <summary>
    /// Creates a new route repository using a collection of routes and route times
    /// </summary>
    /// <param name="routeCollection">Collection of route instances</param>
    /// <param name="routeTimesCollection">Collection of route times instances</param>
    public RouteRepository(IMongoCollection<Route> routeCollection, IMongoCollection<RouteTimes> routeTimesCollection)
    {
        _routeCollection = routeCollection;
        _routeTimesCollection = routeTimesCollection;
    }

    /// <inheritdoc />
    public RouteTimes GetRouteTimesByNameAsync(string routeName)
    {
        return _routeTimesCollection.FindAsync(route => route.Route == routeName).Result.FirstOrDefault();
    }

    /// <inheritdoc />
    public List<Route> GetAllRoutesAsync()
    {
        return _routeCollection.FindAsync(_ => true).Result.ToList();
    }

    /// <inheritdoc />
    public void UpdateRoutes(List<Route> routes)
    {
        foreach (var route in routes)
        {
            UpdateRoute(route);
        }
    }

    /// <inheritdoc />
    public void UpdateRoute(Route route)
    {
        _routeCollection.ReplaceOne(r => r.Name == route.Name, route);
    }
}