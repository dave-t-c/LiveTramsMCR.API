using System.Collections.Generic;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;
using LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Responses;

/// <summary>
///     Response body model for the route planner 'plan journey' endpoint
/// </summary>
public class JourneyPlannerV2ResponseBodyModel
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
    
    /// <summary>
    /// Next service from origin to either the destination or
    /// interchange if a change is required.
    /// </summary>
    public NextServiceIdentifierV2Response NextService { get; init; }
    
    /// <summary>
    /// Service updates, e.g. for engineering works
    /// </summary>
    public HashSet<string> ServiceUpdates { get; init; }
    
    /// <summary>
    /// Fare zones travelled through
    /// </summary>
    public List<int> TravelZones { get; init; }
}