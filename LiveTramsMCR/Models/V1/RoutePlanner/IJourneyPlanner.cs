using LiveTramsMCR.Models.V1.Stops;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
/// Interface that needs to be implemented for any RoutePlanner classes.
/// </summary>
public interface IJourneyPlanner
{
    /// <summary>
    /// Finds a route between an Origin and Destination Stop.
    /// </summary>
    /// <param name="origin">Start of journey Stop</param>
    /// <param name="destination">End of journey Stop</param>
    /// <returns></returns>
    public PlannedJourney PlanJourney(Stop origin, Stop destination);
}