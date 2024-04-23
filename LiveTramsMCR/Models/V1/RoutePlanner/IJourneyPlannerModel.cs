namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
///     Interfaces for Route Planner model classes
/// </summary>
public interface IJourneyPlannerModel
{
    /// <summary>
    ///     Plans a route between an origin and destination
    /// </summary>
    /// <param name="origin">Origin stop TLAREF</param>
    /// <param name="destination">Destination stop TLAREF</param>
    /// <returns>Planned Journey including interchange information</returns>
    public PlannedJourney PlanJourney(string origin, string destination);
}