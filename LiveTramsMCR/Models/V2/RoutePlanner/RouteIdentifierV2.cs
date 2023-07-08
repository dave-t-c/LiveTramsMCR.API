using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner;

/// <summary>
/// Identifies Route related features for journeys such as interchanges.
/// </summary>
public class RouteIdentifierV2
{
    private readonly IRouteRepositoryV2 _routeRepository;
    
    /// <summary>
    /// Initialises a RouteIdentifier with the routes
    /// to use for processing. 
    /// </summary>
    /// <param name="routeRepository">Routes to use for processing.</param>
    public RouteIdentifierV2(IRouteRepositoryV2 routeRepository)
    {
        _routeRepository = routeRepository;
    }

    /// <summary>
    /// Determines if an interchange is required between two stops
    /// by checking to see if there is a route that contains
    /// both the origin and destination stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of journey</param>
    /// <returns>If there is route that contains both Stops</returns>
    public bool IsInterchangeRequired(StopKeysV2 origin, StopKeysV2 destination)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));

        var routes = _routeRepository.GetRoutes();
        //Returns true if there is a route that contains both origin and dest stops.
        //Find returns null if there is not a match, so interchange is required if there is not a match
        return routes.Find(route =>
            route.Stops.Contains(origin) && route.Stops.Contains(destination)) is null;
    }

    /// <summary>
    /// Identifies the Interchange Stop for Stops where
    /// the interchange stop belongs to a route available from the Origin Stop,
    /// and also belongs to a route that belongs to the Destination Stop.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public StopKeysV2 IdentifyInterchangeStop(StopKeysV2 origin, StopKeysV2 destination)
    {
        // N.b. We can take this approach as there is nowhere on the metrolink network that
        // you can go between that cant be completed with a max of 1 interchange, 
        // as it is a hub and spokes network, where the central section (Cornbrook to St Peters Square)
        // acts as the hub.

        if (origin is null)
            throw new ArgumentNullException(nameof(origin));

        if (destination is null)
            throw new ArgumentNullException(nameof(destination));

        var routes = _routeRepository.GetRoutes();
        
        //Identify the routes for a stop.
        var originRoutes = routes.FindAll(route => route.Stops.Contains(origin));
        var destRoutes = routes.FindAll(route => route.Stops.Contains(destination));

        // We need to identify stops that exist on both lines, and then select the 
        // stop closest to the dest stop.
        var stopDistanceFromDestination = new Dictionary<StopKeysV2, int>();
        var stopDistanceFromOrigin = new Dictionary<StopKeysV2, int>();
        foreach (var originRoute in originRoutes)
        {
            foreach (var destRoute in destRoutes)
            {
                var intersectingStops = originRoute.Stops.Intersect(destRoute.Stops).ToList();
                if (!intersectingStops.Any())
                    continue;

                // Identify the distance between the origin and the destination for each of the possible stops.
                foreach (var stop in intersectingStops)
                {
                    stopDistanceFromDestination[stop] = destRoute.GetIntermediateStops(stop, destination).Count + 1;
                }

                foreach (var stop in intersectingStops.Where(stop => !stopDistanceFromOrigin.ContainsKey(stop)))
                {
                    stopDistanceFromOrigin[stop] = originRoute.GetIntermediateStops(origin, stop).Count + 1;
                }
            }
        }
        
        // When there are multiple routes, the interchange stop closest to the 
        // destination is selected.
        var interchangeEntry = stopDistanceFromDestination.MinBy(kvp => kvp.Value);
        
        //Identify all stops with the same distance.
        var minDistanceStops = stopDistanceFromDestination
            .Where(entry => entry.Value == interchangeEntry.Value)
            .Select(entry => entry.Key).ToList();
        
        //If there is only a single entry, we do not need to identify the one the closest to the origin. 
        return minDistanceStops.Count == 1 ? minDistanceStops.First() :
            //Of these stops, identify the stop closest to the route start, to handle cases such as AHN -> BRY
            // Where it can be via St Peters Square or Piccadilly Gardens, which is quicker. 
            minDistanceStops.MinBy(stop => stopDistanceFromOrigin[stop]);
    }

    /// <summary>
    /// Identifies the Routes that connect two stops.
    /// This returns a list of routes that can be taken between two stops.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of journey</param>
    /// <returns>Routes that link the two stops</returns>
    public List<RouteV2> IdentifyRoutesBetween(StopKeysV2 origin, StopKeysV2 destination)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        
        var routes = _routeRepository.GetRoutes();
        
        return routes.FindAll(route => route.Stops.Contains(origin) && route.Stops.Contains(destination));
    }

    

    /// <summary>
    /// Identifies the correct terminus of a route depending on the]
    /// direction of travel between the origin and destination stops.
    /// </summary>
    /// <param name="origin">Origin of journey on route</param>
    /// <param name="destination">Destination of journey on route</param>
    /// <param name="route">Route travelling on</param>
    /// <returns>Terminus Stop for the route direction</returns>
    public StopKeysV2 IdentifyRouteTerminus(StopKeysV2 origin, StopKeysV2 destination, RouteV2 route)
    {
        route.ValidateStopsOnRoute(origin, destination);

        var originIndex = route.Stops.IndexOf(origin);
        var destinationIndex = route.Stops.IndexOf(destination);
        return destinationIndex > originIndex ? route.Stops.Last() : route.Stops.First();
    }

    /// <summary>
    /// Converts a stop keys object to a stop by identifying it on a given route.
    /// </summary>
    /// <param name="stopKeys"></param>
    /// <param name="routeV2"></param>
    /// <returns></returns>
    public static StopV2 ConvertStopKeysToStop(StopKeysV2 stopKeys, RouteV2 routeV2)
    {
        return routeV2.StopsDetail?.FirstOrDefault(stop =>
            stop.StopName == stopKeys.StopName &&
            stop.Tlaref == stopKeys.Tlaref);
    }
}