using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Responses;

/// <summary>
/// Response body model for the route planner 'plan journey' endpoint
/// </summary>
public class RoutePlannerV2ResponseBodyModel
{
    /// <summary>
    /// Journey information such as routes / stops from origin,
    /// and if an interchange is required.
    /// </summary>
    public PlannedJourneyV2 PlannedJourney { get; init; }
}