using System.Collections.Generic;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

namespace LiveTramsMCR.Models.V2.RoutePlanner.TravelZones;

/// <inheritdoc />
public class ZoneIdentifierV2 : IZoneIdentifierV2
{

    /// <inheritdoc />
    public List<int> IdentifyZonesForJourney(PlannedJourneyV2 journeyV2)
    {
        return new List<int>()
        {
            4
        };
    }
}