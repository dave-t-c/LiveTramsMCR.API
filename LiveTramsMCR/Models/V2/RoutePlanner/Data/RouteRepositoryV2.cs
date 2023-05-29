using System.Collections.Generic;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Data;

/// <inheritdoc />
public class RouteRepositoryV2 : IRouteRepositoryV2
{
    private readonly IMongoCollection<RouteV2> _routesCollection;

    /// <summary>
    /// Create a new routes repository using a routes collection.
    /// </summary>
    /// <param name="routesCollection">Collection queried to get relevant data</param>
    public RouteRepositoryV2(IMongoCollection<RouteV2> routesCollection)
    {
        _routesCollection = routesCollection;
    }

    /// <inheritdoc />
    public List<RouteV2> GetRoutes()
    {
        return _routesCollection.FindAsync(_ => true).Result.ToList();
    }
}