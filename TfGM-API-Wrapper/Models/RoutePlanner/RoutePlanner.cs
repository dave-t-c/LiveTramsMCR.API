using System.Collections.Generic;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.RoutePlanner;

/// <summary>
/// Plans routes between Stop objects.
/// </summary>
public class RoutePlanner : IRoutePlanner
{
    /// <summary>
    /// Create a new route planner with a list of available routes.
    /// </summary>
    /// <param name="routes">List of possible routes a journey can take</param>
    public RoutePlanner(List<Route> routes)
    {
        
    }
    
    /// <summary>
    /// Finds a route between an Origin and Destination Stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of Journey</param>
    /// <returns>List of possible Planned Routes</returns>
    public List<PlannedRoute> FindRoute(Stop origin, Stop destination)
    {
        return new List<PlannedRoute>();
    }
}