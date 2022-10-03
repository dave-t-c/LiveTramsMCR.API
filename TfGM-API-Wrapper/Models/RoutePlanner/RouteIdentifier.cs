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
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool IsInterchangeRequired(Stop origin, Stop destination)
    {
        return false;
    }
}