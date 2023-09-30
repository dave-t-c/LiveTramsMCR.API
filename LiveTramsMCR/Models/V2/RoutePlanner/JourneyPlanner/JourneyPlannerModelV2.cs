using System;
using System.Linq;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V2.RoutePlanner.Responses;
using LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;
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
    private readonly INextServiceIdentifierV2 _nextServiceIdentifierV2;
    private readonly ServiceProcessor _serviceProcessor;

    /// <summary>
    ///     Creates a new journey planner model
    /// </summary>
    public JourneyPlannerModelV2(
        StopLookupV2 stopLookupV2,
        IJourneyPlannerV2 journeyPlannerV2,
        IJourneyVisualiserV2 journeyVisualiserV2,
        INextServiceIdentifierV2 nextServiceIdentifierV2,
        ServiceProcessor serviceProcessor)
    {
        _stopLookupV2 = stopLookupV2;
        _journeyPlannerV2 = journeyPlannerV2;
        _journeyVisualiserV2 = journeyVisualiserV2;
        _nextServiceIdentifierV2 = nextServiceIdentifierV2;
        _serviceProcessor = serviceProcessor;
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
        
        var services = _serviceProcessor.RequestServices(plannedJourney);
        var nextService = IdentifyNextService(services, plannedJourney);
        
        return new RoutePlannerV2ResponseBodyModel
        {
            PlannedJourney = plannedJourney,
            VisualisedJourney = visualisedJourney
        };
    }

    private NextServiceIdentifierV2Response IdentifyNextService(
        FormattedServices services,
        PlannedJourneyV2 plannedJourney)
    {
        var destinationStop = plannedJourney.RequiresInterchange ? 
            plannedJourney.InterchangeStop : plannedJourney.DestinationStop;

        var allServices = services.Destinations.SelectMany(x => x.Value);
        var filteredServices = allServices.Where(
            service => service.SourceTlaref == plannedJourney.OriginStop.Tlaref).ToList();
        
        var request = new NextServiceIdentifierV2Request()
        {
            Origin = plannedJourney.OriginStop,
            Destination = destinationStop,
            Services = filteredServices,
            Routes = plannedJourney.RoutesFromOrigin
        };
        var response = this._nextServiceIdentifierV2.IdentifyNextService(request);
        return response;
    }
}