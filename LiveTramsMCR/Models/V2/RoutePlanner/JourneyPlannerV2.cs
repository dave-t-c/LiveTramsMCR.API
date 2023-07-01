using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner;

/// <summary>
/// Plans routes between Stop objects.
/// </summary>
public class JourneyPlannerV2 : IJourneyPlannerV2
{
    private readonly RouteIdentifierV2 _routeIdentifierV2;
    private readonly JourneyTimeFinder _journeyTimeFinder;

    /// <summary>
    /// Create a new route planner with a list of available routes.
    /// </summary>
    /// <param name="routeRepositoryV1"></param>
    /// <param name="routeRepositoryV2">Repository for retrieving route times</param>
    public JourneyPlannerV2(IRouteRepository routeRepositoryV1, IRouteRepositoryV2 routeRepositoryV2)
    {
        _routeIdentifierV2 = new RouteIdentifierV2(routeRepositoryV2);
        _journeyTimeFinder = new JourneyTimeFinder(routeRepositoryV1);
    }
    
    /// <summary>
    /// Finds a route between an Origin and Destination Stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of Journey</param>
    /// <returns>List of possible Planned Routes</returns>
    public PlannedJourneyV2 PlanJourney(StopV2 origin, StopV2 destination)
    {
        var originStopKeys = new StopKeysV2
        {
            StopName = origin.StopName, Tlaref = origin.Tlaref
        };

        var destinationStopKeys = new StopKeysV2
        {
            StopName = destination.StopName, Tlaref = destination.Tlaref
        };
        
        var interchangeIsRequired = _routeIdentifierV2.IsInterchangeRequired(originStopKeys, destinationStopKeys);
        var plannedJourney = interchangeIsRequired
            ? PlanJourneyWithInterchange(originStopKeys, destinationStopKeys)
            : PlanJourneyWithoutInterchange(originStopKeys, destinationStopKeys);

        plannedJourney.RequiresInterchange = interchangeIsRequired;
        plannedJourney.OriginStop = origin;
        plannedJourney.DestinationStop = destination;
        plannedJourney.TotalJourneyTimeMinutes =
            plannedJourney.MinutesFromOrigin + plannedJourney.MinutesFromInterchange;
        return plannedJourney;
    }


    /// <summary>
    /// Plans a journey where it is known an interchange is not required.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of journey</param>
    /// <returns>Planned journey without an interchange between origin and destination stops</returns>
    private PlannedJourneyV2 PlanJourneyWithoutInterchange(StopKeysV2 origin, StopKeysV2 destination)
    {
        var originRoutes = _routeIdentifierV2.IdentifyRoutesBetween(origin, destination);
        var originStops = originRoutes.First().GetIntermediateStops(origin, destination);
        var terminiFromOrigin = new HashSet<StopKeysV2>();
        foreach (var route in originRoutes)
        {
            terminiFromOrigin.Add(RouteIdentifierV2.IdentifyRouteTerminus(origin, destination, route));
        }

        var minutesFromOrigin = IdentifyJourneyTime(originRoutes.First(), origin, destination);
        return new PlannedJourneyV2
        {
            RoutesFromOrigin = originRoutes,
            StopsFromOrigin = originStops,
            TerminiFromOrigin = terminiFromOrigin,
            MinutesFromOrigin = minutesFromOrigin
        };
    }
    
    /// <summary>
    /// Plans a journey where it is known an interchange is required.
    /// This identifies and handles the interchange stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of journey</param>
    /// <returns>Journey information including relevant </returns>
    private PlannedJourneyV2 PlanJourneyWithInterchange(StopKeysV2 origin, StopKeysV2 destination)
    {
        var interchangeStopKeys = _routeIdentifierV2.IdentifyInterchangeStop(origin, destination);
        var originRoutes = _routeIdentifierV2.IdentifyRoutesBetween(origin, interchangeStopKeys);
        var originStops = originRoutes.First().GetIntermediateStops(origin, interchangeStopKeys);

        var terminiFromOrigin = new HashSet<StopKeysV2>();
        foreach (var route in originRoutes)
        {
            terminiFromOrigin.Add(RouteIdentifierV2.IdentifyRouteTerminus(origin, interchangeStopKeys, route));
        }


        var interchangeRoutes = _routeIdentifierV2.IdentifyRoutesBetween(interchangeStopKeys, destination);
        var interchangeStops = interchangeRoutes.First().GetIntermediateStops(interchangeStopKeys, destination);
        var interchangeStop = RouteIdentifierV2.ConvertStopKeysToStop(interchangeStopKeys, interchangeRoutes.First());
        
        var terminiFromInterchange = new HashSet<StopKeysV2>();
        foreach (var route in interchangeRoutes)
        {
            terminiFromInterchange.Add(
                RouteIdentifierV2.IdentifyRouteTerminus(
                    interchangeStopKeys, 
                    destination, 
                    route)
                );
        }

        var minutesFromOrigin = IdentifyJourneyTime(originRoutes.First(), origin, interchangeStopKeys);
        var minutesFromInterchange = IdentifyJourneyTime(interchangeRoutes.First(),
            interchangeStopKeys, destination);
        
        
        return new PlannedJourneyV2
        {
            InterchangeStop = interchangeStop,
            RoutesFromOrigin = originRoutes,
            RoutesFromInterchange = interchangeRoutes,
            StopsFromOrigin = originStops,
            StopsFromInterchange = interchangeStops,
            TerminiFromOrigin = terminiFromOrigin,
            TerminiFromInterchange = terminiFromInterchange,
            MinutesFromOrigin = minutesFromOrigin,
            MinutesFromInterchange = minutesFromInterchange
        };
    }

    /// <summary>
    /// Identifies the journey time between an origin and interchange / destination stop.
    /// </summary>
    /// <param name="route">Route being taken</param>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">Destination / Interchange of journey</param>
    /// <returns>Integer of minutes between origin and destination</returns>
    private int IdentifyJourneyTime(RouteV2 route, StopKeysV2 origin, StopKeysV2 destination)
    {
        return _journeyTimeFinder.FindJourneyTime(route.Name,
            origin.StopName, destination.StopName);
    }
}