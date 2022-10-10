namespace TfGM_API_Wrapper.Models.RoutePlanner;

/// <summary>
/// Interfaces for Route Planner model classes
/// </summary>
public interface IJourneyPlannerModel
{
    /// <summary>
    /// Plans a route between an origin and destination
    /// </summary>
    /// <param name="origin">Origin stop name or TLAREF</param>
    /// <param name="destination">Destination stop name or TLAREF</param>
    /// <returns>Planned Journey including interchange information</returns>
    public PlannedJourney PlanJourney(string origin, string destination);
}