using System;
using System.Collections.Generic;
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
        
        //Returns true if there is a route that contains both origin and dest stops.
        //Find returns null if there is not a match, so interchange is required if there is not a match
        return _routes.Find(route => route.Stops.Contains(origin) && route.Stops.Contains(destination)) is null;
    }
}