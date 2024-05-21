using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Configuration;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V1.RoutePlanner.Data;

/// <inheritdoc />
public class RouteRepository : IRouteRepository
{
    private readonly IMongoCollection<Route> _routeCollection;

    private readonly IMongoCollection<RouteTimes> _routeTimesCollection;

    private readonly IDynamoDBContext _context;

    /// <summary>
    ///     Creates a new route repository using a collection of routes and route times
    /// </summary>
    public RouteRepository(
        IMongoCollection<Route> routeCollection, 
        IMongoCollection<RouteTimes> routeTimesCollection, 
        IDynamoDBContext context)
    {
        _routeCollection = routeCollection;
        _routeTimesCollection = routeTimesCollection;
        _context = context;
    }

    /// <inheritdoc />
    public RouteTimes GetRouteTimesByNameAsync(string routeName)
    {
        RouteTimes result;
        if (FeatureFlags.DynamoDbEnabled)
        {
            result = _context.LoadAsync<RouteTimes>(routeName).Result;
        }
        else
        {
            result = _routeTimesCollection.FindAsync(route => route.Route == routeName).Result.FirstOrDefault(); 
        }

        return result;
    }

    /// <inheritdoc />
    public List<RouteTimes> GetAllRouteTimes()
    {
        List<RouteTimes> result;
        if (FeatureFlags.DynamoDbEnabled)
        {
            result = _context.ScanAsync<RouteTimes>(default).GetRemainingAsync().Result;
        }
        else
        {
            result = _routeTimesCollection.FindAsync(_ => true).Result.ToList();
        }

        return result;
    }

    /// <inheritdoc />
    public List<Route> GetAllRoutes()
    {
        List<Route> result;
        if (FeatureFlags.DynamoDbEnabled)
        {
            result = _context.ScanAsync<Route>(default).GetRemainingAsync().Result;
        }
        else
        {
            result = _routeCollection.FindAsync(_ => true).Result.ToList(); 
        }

        return result;
    }
}