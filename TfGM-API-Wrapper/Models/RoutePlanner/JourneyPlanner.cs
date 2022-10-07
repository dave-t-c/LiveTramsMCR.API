using System.Collections.Generic;
using System.Linq;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.RoutePlanner;

/// <summary>
/// Plans routes between Stop objects.
/// </summary>
public class JourneyPlanner : IJourneyPlanner
{
    private List<Route> _routes;
    private RouteIdentifier _routeIdentifier;
    
    /// <summary>
    /// Create a new route planner with a list of available routes.
    /// </summary>
    /// <param name="routes">List of possible routes a journey can take</param>
    public JourneyPlanner(List<Route> routes)
    {
        _routes = routes;
        _routeIdentifier = new RouteIdentifier(_routes);
    }
    
    /// <summary>
    /// Finds a route between an Origin and Destination Stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of Journey</param>
    /// <returns>List of possible Planned Routes</returns>
    public PlannedJourney PlanJourney(Stop origin, Stop destination)
    {
        var interchangeIsRequired = _routeIdentifier.IsInterchangeRequired(origin, destination);
        var plannedJourney = interchangeIsRequired
            ? PlanJourneyWithInterchange(origin, destination)
            : PlanJourneyWithoutInterchange(origin, destination);

        plannedJourney.RequiresInterchange = interchangeIsRequired;
        plannedJourney.OriginStop = origin;
        plannedJourney.DestinationStop = destination;
        return plannedJourney;
    }


    private PlannedJourney PlanJourneyWithoutInterchange(Stop origin, Stop destination)
    {
        var originRoutes = _routeIdentifier.IdentifyRoutesBetween(origin, destination);
        var originStops = _routeIdentifier
            .IdentifyIntermediateStops(origin, destination, originRoutes.First());
        var terminiFromOrigin = new HashSet<Stop>();
        foreach (var route in originRoutes)
        {
            terminiFromOrigin.Add(_routeIdentifier
                .IdentifyRouteTerminus(origin, destination, route));
        }
        return new PlannedJourney
        {
            RoutesFromOrigin = originRoutes,
            StopsFromOrigin = originStops,
            TerminiFromOrigin = terminiFromOrigin
        };
    }
    
    private PlannedJourney PlanJourneyWithInterchange(Stop origin, Stop destination)
    {
        var interchangeStop = _routeIdentifier.IdentifyInterchangeStop(origin, destination);
        var originRoutes = _routeIdentifier.IdentifyRoutesBetween(origin, interchangeStop);
        var originStops = _routeIdentifier
            .IdentifyIntermediateStops(origin, interchangeStop, originRoutes.First());
        var terminiFromOrigin = new HashSet<Stop>();
        foreach (var route in originRoutes)
        {
            terminiFromOrigin.Add(_routeIdentifier
                .IdentifyRouteTerminus(origin, interchangeStop, route));
        }


        var interchangeRoutes = _routeIdentifier.IdentifyRoutesBetween(interchangeStop, destination);
        var interchangeStops = _routeIdentifier
            .IdentifyIntermediateStops(interchangeStop, destination, interchangeRoutes.First());
        var terminiFromInterchange = new HashSet<Stop>();
        foreach (var route in interchangeRoutes)
        {
            terminiFromInterchange.Add(_routeIdentifier
                .IdentifyRouteTerminus(interchangeStop, destination, route));
        }

        
        return new PlannedJourney
        {
            InterchangeStop = interchangeStop,
            RoutesFromOrigin = originRoutes,
            RoutesFromInterchange = interchangeRoutes,
            StopsFromOrigin = originStops,
            StopsFromInterchange = interchangeStops,
            TerminiFromOrigin = terminiFromOrigin,
            TerminiFromInterchange = terminiFromInterchange
        };
    }
}