using System.Collections.Generic;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.RoutePlanner;

/// <summary>
/// Plans routes between Stop objects.
/// </summary>
public class JourneyPlanner : IJourneyPlanner
{
    /// <summary>
    /// Create a new route planner with a list of available routes.
    /// </summary>
    /// <param name="routes">List of possible routes a journey can take</param>
    public JourneyPlanner(List<Route> routes)
    {
        
    }
    
    /// <summary>
    /// Finds a route between an Origin and Destination Stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of Journey</param>
    /// <returns>List of possible Planned Routes</returns>
    public PlannedJourney PlanJourney(Stop origin, Stop destination)
    {
        var purpleRoute = new Route("Purple", "", new List<Stop>());
        var greenRoute = new Route("Green", "", new List<Stop>());
        var routesFromOrigin = new List<Route> {purpleRoute, greenRoute};
        return new PlannedJourney
        {
            RoutesFromOrigin = routesFromOrigin
        };
    }
}