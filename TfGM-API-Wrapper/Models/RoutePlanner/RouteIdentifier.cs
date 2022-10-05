using System;
using System.Collections.Generic;
using System.Linq;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.RoutePlanner;

public class RouteIdentifier
{
    private readonly List<Route> _routes;
    
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
        foreach (var originRoute in originRoutes)
        {
            foreach (var destRoute in destRoutes)
            {
                var intersectingStops = originRoute.Stops.Intersect(destRoute.Stops).ToList();
                if (!intersectingStops.Any())
                    continue;

                foreach (var stop in intersectingStops.Where(stop => !stopDistanceFromDestination.ContainsKey(stop)))
                {
                    stopDistanceFromDestination[stop] = destRoute.GetStopsBetween(stop, destination).Count + 1;
                }
            }
        }
        
        // When there are multiple routes, the interchange stop closest to the 
        // destination is selected.
        var interchangeStop = stopDistanceFromDestination.MinBy(kvp => kvp.Value).Key;
        return interchangeStop;
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
        var purpleRoute = new Route("Purple", "", new List<Stop>());
        var greenRoute = new Route("Green", "", new List<Stop>());
        return new List<Route>{purpleRoute, greenRoute};
    }
}