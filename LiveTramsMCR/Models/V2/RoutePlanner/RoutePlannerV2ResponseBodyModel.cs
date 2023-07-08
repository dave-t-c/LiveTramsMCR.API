namespace LiveTramsMCR.Models.V2.RoutePlanner;

/// <summary>
/// Response body model for the route planner 'plan journey' endpoint
/// </summary>
public class RoutePlannerV2ResponseBodyModel
{
    public PlannedJourneyV2 PlannedJourney { get; set; }
}