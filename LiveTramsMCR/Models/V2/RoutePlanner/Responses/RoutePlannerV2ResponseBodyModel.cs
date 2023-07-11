using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Responses;

/// <summary>
///     Response body model for the route planner 'plan journey' endpoint
/// </summary>
public class RoutePlannerV2ResponseBodyModel
{
    /// <summary>
    ///     Journey information such as routes / stops from origin,
    ///     and if an interchange is required.
    /// </summary>
    public PlannedJourneyV2 PlannedJourney { get; init; }
    
    /// <summary>
    /// Visualisation information for route maps.
    /// </summary>
    public VisualisedJourneyV2 VisualisedJourney { get; init; }
}