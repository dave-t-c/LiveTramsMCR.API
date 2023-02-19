using System;
using LiveTramsMCR.Models.Resources;
using LiveTramsMCR.Models.Stops;

namespace LiveTramsMCR.Models.RoutePlanner;

/// <summary>
/// Data model for the Journey Planner package.
/// This identifies the used stops and uses the Journey planner
/// class to plan a journey.
/// </summary>
public class JourneyPlannerModel: IJourneyPlannerModel
{
    private readonly IJourneyPlanner _journeyPlanner;
    private readonly StopLookup _stopLookup;
    
    /// <summary>
    /// Creates a journey planner model that can be used
    /// for planning journeys between stops.
    /// </summary>
    /// <param name="importedResources"></param>
    /// <param name="journeyPlanner"></param>
    public JourneyPlannerModel(ImportedResources importedResources, IJourneyPlanner journeyPlanner)
    {
        _journeyPlanner = journeyPlanner;
        _stopLookup = new StopLookup(importedResources);
    }
    
    
    /// <summary>
    /// Plans a journey between an origin stop name or TLAREF and
    /// a destination stop name or TLAREF, identifying any relevant interchange information.
    /// </summary>
    /// <param name="origin">Journey start stop name or TLAREF</param>
    /// <param name="destination">Journey end stop name or TLAREF</param>
    /// <returns>Planned journey including relevant interchange information</returns>
    public PlannedJourney PlanJourney(string origin, string destination)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        var originStop = _stopLookup.LookupStop(origin);
        var destinationStop = _stopLookup.LookupStop(destination);
        return _journeyPlanner?.PlanJourney(originStop, destinationStop);
    }
}