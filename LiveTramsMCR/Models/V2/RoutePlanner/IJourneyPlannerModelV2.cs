namespace LiveTramsMCR.Models.V2.RoutePlanner;

public interface IJourneyPlannerModelV2
{
    /// <summary>
    /// Plans a route between an origin and destination
    /// </summary>
    /// <param name="origin">Origin stop name or TLAREF</param>
    /// <param name="destination">Destination stop name or TLAREF</param>
    /// <returns>Planned Journey including interchange information</returns>
    public PlannedJourneyV2 PlanJourney(string origin, string destination);
}