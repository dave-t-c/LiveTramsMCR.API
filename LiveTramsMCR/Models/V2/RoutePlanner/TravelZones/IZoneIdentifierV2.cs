using System.Collections.Generic;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

namespace LiveTramsMCR.Models.V2.RoutePlanner.TravelZones;

/// <summary>
/// Identifies zones travelled through for journeys.
/// </summary>
public interface IZoneIdentifierV2
{
    /// <summary>
    /// Identify zones travelled through for a PlannedJourneyV2
    /// </summary>
    /// <returns>Ascending list of zones travelled through</returns>
    public List<int> IdentifyZonesForJourney(PlannedJourneyV2 journeyV2);
}