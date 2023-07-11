using System.Linq;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;

/// <inheritdoc />
public class JourneyVisualiserV2 : IJourneyVisualiserV2
{

    /// <inheritdoc/> 
    public VisualisedJourneyV2 VisualiseJourney(PlannedJourneyV2 plannedJourneyV2)
    {
        return plannedJourneyV2.RequiresInterchange ? VisualiseJourneyWithInterchange(plannedJourneyV2)
            : VisualiseJourneyWithoutInterchange(plannedJourneyV2);
    }

    private static VisualisedJourneyV2 VisualiseJourneyWithoutInterchange(PlannedJourneyV2 plannedJourneyV2)
    {
        var route = plannedJourneyV2.RoutesFromOrigin.First();
        var originStopKeys = new StopKeysV2()
        {
            StopName = plannedJourneyV2.OriginStop.StopName, Tlaref = plannedJourneyV2.OriginStop.Tlaref
        };

        var destinationStopKeys = new StopKeysV2()
        {
            StopName = plannedJourneyV2.DestinationStop.StopName, Tlaref = plannedJourneyV2.DestinationStop.Tlaref
        };

        var polylineFromOrigin = route.GetPolylineBetweenStops(originStopKeys, destinationStopKeys);
        return new VisualisedJourneyV2()
        {
            PolylineFromOrigin = polylineFromOrigin
        };
    }
    
    private static VisualisedJourneyV2 VisualiseJourneyWithInterchange(PlannedJourneyV2 plannedJourneyV2)
    {
        var routeFromOrigin = plannedJourneyV2.RoutesFromOrigin.First();
        var routeFromInterchange = plannedJourneyV2.RoutesFromInterchange.First();
        var originStopKeys = new StopKeysV2()
        {
            StopName = plannedJourneyV2.OriginStop.StopName, Tlaref = plannedJourneyV2.OriginStop.Tlaref
        };

        var interchangeStopKeys = new StopKeysV2()
        {
            StopName = plannedJourneyV2.InterchangeStop.StopName, Tlaref = plannedJourneyV2.InterchangeStop.Tlaref
        };

        var destinationStopKeys = new StopKeysV2()
        {
            StopName = plannedJourneyV2.DestinationStop.StopName, Tlaref = plannedJourneyV2.DestinationStop.Tlaref
        };

        var polylineFromOrigin = routeFromOrigin.GetPolylineBetweenStops(originStopKeys, interchangeStopKeys);
        var polylineFromInterchange = routeFromInterchange.GetPolylineBetweenStops(interchangeStopKeys, destinationStopKeys);
        return new VisualisedJourneyV2()
        {
            PolylineFromOrigin = polylineFromOrigin,
            PolylineFromInterchange = polylineFromInterchange
        };
    }
}