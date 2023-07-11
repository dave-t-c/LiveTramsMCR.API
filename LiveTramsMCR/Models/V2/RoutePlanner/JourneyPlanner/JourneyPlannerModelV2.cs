using System;
using LiveTramsMCR.Models.V2.RoutePlanner.Responses;
using LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

/// <summary>
///     Model for planning journeys using stops and routes v2
/// </summary>
public class JourneyPlannerModelV2 : IJourneyPlannerModelV2
{
    private readonly IJourneyVisualiserV2 _journeyVisualiserV2;
    private readonly IJourneyPlannerV2 _journeyPlannerV2;
    private readonly StopLookupV2 _stopLookupV2;

    /// <summary>
    ///     Creates a new journey planner model
    /// </summary>
    public JourneyPlannerModelV2(
        StopLookupV2 stopLookupV2,
        IJourneyPlannerV2 journeyPlannerV2,
        IJourneyVisualiserV2 journeyVisualiserV2)
    {
        _stopLookupV2 = stopLookupV2;
        _journeyPlannerV2 = journeyPlannerV2;
        _journeyVisualiserV2 = journeyVisualiserV2;
    }

    /// <inheritdoc />
    public RoutePlannerV2ResponseBodyModel PlanJourney(string origin, string destination)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        var originStop = _stopLookupV2.LookupStop(origin);
        var destinationStop = _stopLookupV2.LookupStop(destination);
        var plannedJourney = _journeyPlannerV2.PlanJourney(originStop, destinationStop);
        var visualisedJourney = _journeyVisualiserV2.VisualiseJourney(plannedJourney);
        
        return new RoutePlannerV2ResponseBodyModel
        {
            PlannedJourney = plannedJourney,
            VisualisedJourney = visualisedJourney
        };
    }
}