using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.Stops;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
/// Identifies Route related features for journeys such as interchanges.
/// </summary>
public class RouteIdentifier
{
    private readonly List<Route> _routes;
    
    /// <summary>
    /// Initialises a RouteIdentifier with the routes
    /// to use for processing. 
    /// </summary>
    /// <param name="routes">Routes to use for processing.</param>
    public RouteIdentifier(List<Route> routes)
    {
        _routes = routes;
    }

    /// <summary>
    /// Determines if an interchange is required between two stops
    /// by checking to see if there is a route that contains
    /// both the origin and destination stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of journey</param>
    /// <returns>If there is route that contains both Stops</returns>
    public bool IsInterchangeRequired(Stop origin, Stop destination)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        
        //Returns true if there is a route that contains both origin and dest stops.
        //Find returns null if there is not a match, so interchange is required if there is not a match
        return _routes.Find(route => route.Stops.Contains(origin) && route.Stops.Contains(destination)) is null;
    }

    /// <summary>
    /// Identifies the Interchange Stop for Stops where
    /// the interchange stop belongs to a route available from the Origin Stop,
    /// and also belongs to a route that belongs to the Destination Stop.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public Stop IdentifyInterchangeStop(Stop origin, Stop destination)
    {
        // N.b. We can take this approach as there is nowhere on the metrolink network that
        // you can go between that cant be completed with a max of 1 interchange, 
        // as it is a hub and spokes network, where the central section (Cornbrook to St Peters Square)
        // acts as the hub.

        if (origin is null)
            throw new ArgumentNullException(nameof(origin));

        if (destination is null)
            throw new ArgumentNullException(nameof(destination));

        //Identify the routes for a stop.
        var originRoutes = _routes.FindAll(route => route.ContainsStop(origin));
        var destRoutes = _routes.FindAll(route => route.ContainsStop(destination));

        // We need to identify stops that exist on both lines, and then select the 
        // stop closest to the dest stop.
        var stopDistanceFromDestination = new Dictionary<Stop, int>();
        var stopDistanceFromOrigin = new Dictionary<Stop, int>();
        foreach (var originRoute in originRoutes)
        {
            foreach (var destRoute in destRoutes)
            {
                var intersectingStops = originRoute.Stops.Intersect(destRoute.Stops).ToList();
                if (!intersectingStops.Any())
                    continue;

                // Identify the distance between the origin and the destination for each of the possible stops.
                foreach (var stop in intersectingStops.Where(stop => !stopDistanceFromDestination.ContainsKey(stop)))
                {
                    stopDistanceFromDestination[stop] = destRoute.GetStopsBetween(stop, destination).Count + 1;
                }

                foreach (var stop in intersectingStops.Where(stop => !stopDistanceFromOrigin.ContainsKey(stop)))
                {
                    stopDistanceFromOrigin[stop] = originRoute.GetStopsBetween(origin, stop).Count + 1;
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
    public List<Route> IdentifyRoutesBetween(Stop origin, Stop destination)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        return _routes.FindAll(route => route.ContainsStop(origin) && route.ContainsStop(destination));
    }

    /// <summary>
    /// Identifies Stops between an Origin and Destination stop.
    /// This excludes the origin and destination stop.
    /// </summary>
    /// <param name="origin">Stop to start at.</param>
    /// <param name="destination">Stop to end at.</param>
    /// <param name="route">Route to identify intermediate stops on.</param>
    /// <returns>List of intermediate stops between origin and destination on given route.</returns>
    public List<Stop> IdentifyIntermediateStops(Stop origin, Stop destination, Route route)
    {
        ValidateStopsOnRoute(origin, destination, route);
        
        var originIndex = route.Stops.IndexOf(origin);
        var destinationIndex = route.Stops.IndexOf(destination);
        var increment = destinationIndex > originIndex ? 1 : -1;

        var intermediateStops = new List<Stop>();
        for (var i = originIndex + increment; i != destinationIndex; i += increment)
        {
            intermediateStops.Add(route.Stops[i]);
        }

        return intermediateStops;
    }

    /// <summary>
    /// Identifies the correct terminus of a route depending on the]
    /// direction of travel between the origin and destination stops.
    /// </summary>
    /// <param name="origin">Origin of journey on route</param>
    /// <param name="destination">Destination of journey on route</param>
    /// <param name="route">Route travelling on</param>
    /// <returns>Terminus Stop for the route direction</returns>
    public Stop IdentifyRouteTerminus(Stop origin, Stop destination, Route route)
    {
        ValidateStopsOnRoute(origin, destination, route);

        var originIndex = route.Stops.IndexOf(origin);
        var destinationIndex = route.Stops.IndexOf(destination);
        return destinationIndex > originIndex ? route.Stops.Last() : route.Stops.First();
    }

    /// <summary>
    /// Validates the stops being used on the given route.
    /// This returns true if the stops are both valid and exist on the provided route.
    /// </summary>
    /// <param name="origin">Origin Stop to validate</param>
    /// <param name="destination">Destination Stop to validate</param>
    /// <param name="route">Route to validate the Stops exist on</param>
    /// <exception cref="ArgumentNullException">Thrown if any argument is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if either of the stops do not exist on the provided route</exception>
    private static void ValidateStopsOnRoute(Stop origin, Stop destination, Route route)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        if (route is null)
            throw new ArgumentNullException(nameof(route));
        
        if (!route.Stops.Contains(origin))
            throw new InvalidOperationException(origin.StopName + " does not exist on " + route.Name + " route");
        if (!route.Stops.Contains(destination))
            throw new InvalidOperationException(destination.StopName + " does not exist on " + route.Name + " route");
    }
}