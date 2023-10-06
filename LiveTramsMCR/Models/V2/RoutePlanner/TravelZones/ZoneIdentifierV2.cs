using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

namespace LiveTramsMCR.Models.V2.RoutePlanner.TravelZones;

/// <inheritdoc />
public class ZoneIdentifierV2 : IZoneIdentifierV2
{

    /// <inheritdoc />
    public List<int> IdentifyZonesForJourney(PlannedJourneyV2 journeyV2)
    {
        if (journeyV2 == null) throw new ArgumentNullException(nameof(journeyV2));
        
        var identifiedZones = new List<string>
        {
            journeyV2.OriginStop.StopZone
        };

        var zonesFromOrigin = journeyV2.StopsFromOrigin.Select(stop => stop.StopZone);
        
        identifiedZones.AddRange(zonesFromOrigin);

        if (journeyV2.RequiresInterchange)
        {
            identifiedZones.Add(journeyV2.InterchangeStop.StopZone);

            var zonesFromInterchange = journeyV2.StopsFromInterchange
                .Select(stop => stop.StopZone);
            
            identifiedZones.AddRange(zonesFromInterchange);
        }
        
        identifiedZones.Add(journeyV2.DestinationStop.StopZone);
        
        // Remove any stops with a/b in, e.g. 3/4
        identifiedZones.RemoveAll(zone => zone.Contains('/'));

        var intZonesList = identifiedZones.Select(int.Parse);
        var distinctZones = intZonesList.Distinct();
        var orderedZones = distinctZones.OrderBy(v => v).ToList();
        
        return orderedZones;
    }
}