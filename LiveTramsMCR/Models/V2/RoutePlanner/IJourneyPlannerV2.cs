using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner;

/// <summary>
/// Interface that needs to be implemented for any RoutePlanner classes.
/// </summary>
public interface IJourneyPlannerV2
{
    /// <summary>
    /// Finds a route between an Origin and Destination Stop.
    /// </summary>
    /// <param name="origin">Start of journey Stop</param>
    /// <param name="destination">End of journey Stop</param>
    /// <returns></returns>
    public PlannedJourneyV2 PlanJourney(StopV2 origin, StopV2 destination);
}