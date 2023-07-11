using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;

/// <summary>
/// Handles all processing required for journey visualisation.
/// </summary>
public interface IJourneyVisualiserV2
{
    /// <summary>
    /// Creates polylines required for visualisation a journey
    /// </summary>
    /// <param name="plannedJourneyV2">Pre-planned journey to visualise</param>
    public VisualisedJourneyV2 VisualiseJourney(PlannedJourneyV2 plannedJourneyV2);
}