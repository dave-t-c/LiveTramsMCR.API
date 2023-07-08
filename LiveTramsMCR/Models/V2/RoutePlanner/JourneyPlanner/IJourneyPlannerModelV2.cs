using LiveTramsMCR.Models.V2.RoutePlanner.Responses;

namespace LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

/// <summary>
/// Interface used for journey planning functionality
/// </summary>
public interface IJourneyPlannerModelV2
{
    /// <summary>
    /// Plans a route between an origin and destination
    /// </summary>
    /// <param name="origin">Origin stop name or TLAREF</param>
    /// <param name="destination">Destination stop name or TLAREF</param>
    /// <returns>Planned Journey including interchange information</returns>
    public RoutePlannerV2ResponseBodyModel PlanJourney(string origin, string destination);
}