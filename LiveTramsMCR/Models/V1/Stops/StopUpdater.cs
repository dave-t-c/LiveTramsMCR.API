using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V1.Stops.Data;

namespace LiveTramsMCR.Models.V1.Stops;

/// <summary>
/// Updates the IDs for stops in the stop repository
/// </summary>
public class StopUpdater
{
    private readonly IStopsRepository _stopsRepository;
    private readonly IRouteRepository _routeRepository;

    /// <summary>
    /// Updates stops into 
    /// </summary>
    /// <param name="stopsRepository"></param>
    /// <param name="routeRepository"></param>
    public StopUpdater(IStopsRepository stopsRepository, IRouteRepository routeRepository)
    {
        _stopsRepository = stopsRepository;
        _routeRepository = routeRepository;
    }

    /// <summary>
    /// Updates the stops in both the stops and route repositories using a
    /// list of unformatted services to take IDs from.
    /// </summary>
    /// <param name="services"></param>
    public void UpdateStopIdsFromServices(List<UnformattedServices> services)
    {
        var stops = _stopsRepository.GetAll();
        foreach (var stop in stops)
        {
            UpdateStop(stop, services);
        }
        
        _stopsRepository.UpdateStops(stops);

        var routes = _routeRepository.GetAllRoutesAsync();
        foreach (var route in routes)
        {
            UpdateRouteStops(route, stops);
        }
        
        _routeRepository.UpdateRoutes(routes);
    }
    
    private static void UpdateStop(Stop stop, List<UnformattedServices> services)
    {
        var ids = services.Where(s => s.Tlaref == stop.Tlaref)
            .Select(s => s.Id)
            .ToList();
        stop.Ids = ids;
    }
    
    private static void UpdateRouteStops(Route route, IReadOnlyCollection<Stop> updatedStops)
    {
        // Get the existing stops.
        // For each stop, find the updated replacement
        // Create a new list 
        var updatedRouteStops = new List<Stop>();
        var existingStops = route.Stops;
        foreach (var stop in existingStops)
        {
            var updatedStop = updatedStops.FirstOrDefault(s => s.Tlaref == stop.Tlaref);
            if (updatedStop is null)
            {
                throw new InvalidOperationException($"Updated stop for tlaref '{stop.Tlaref}' not found");
            }
        
            updatedRouteStops.Add(updatedStop);
        }

        route.Stops = updatedRouteStops;
    }
}