using System.Collections.Generic;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

/// <summary>
///     POCO object that details a planned route between an origin and destination stop.
/// </summary>
public class PlannedJourneyV2
{
    /// <summary>
    ///     Stop the route originates / starts at.
    /// </summary>
    public StopV2 OriginStop { get; set; }

    /// <summary>
    ///     End stop for the route
    /// </summary>
    public StopV2 DestinationStop { get; set; }

    /// <summary>
    ///     Stop where an interchange needs to be made to
    ///     continue on the route. This may be null.
    /// </summary>
    public StopV2 InterchangeStop { get; set; }

    /// <summary>
    ///     Routes taken from the origin destination
    ///     to either the interchange stop or destination
    ///     stop if no interchange is required
    ///     (Only require a single route).
    /// </summary>
    public List<RouteV2> RoutesFromOrigin { get; set; }

    /// <summary>
    ///     Stops between the origin stop and interchange or destination for each route.
    /// </summary>
    public List<StopV2> StopsFromOrigin { get; set; }

    /// <summary>
    ///     Maps the route name to the destination of the Tram to the Interchange Stop or the Destination Stop
    ///     E.g.Green line towards Bury (With the Stop value for Bury)
    /// </summary>
    public HashSet<StopKeysV2> TerminiFromOrigin { get; set; }

    /// <summary>
    ///     Route from the Interchange stop to the
    ///     destination stop. This may be null.
    /// </summary>
    public List<RouteV2> RoutesFromInterchange { get; set; }

    /// <summary>
    ///     Stops between the interchange and destination stop.
    /// </summary>
    public List<StopV2> StopsFromInterchange { get; set; }

    /// <summary>
    ///     Maps the route name to the destination of the Tram from the interchange stop.
    ///     E.g.Green line towards Bury (With the Stop value for Bury)
    /// </summary>
    public HashSet<StopKeysV2> TerminiFromInterchange { get; set; }


    /// <summary>
    ///     Boolean showing if the route requires an
    ///     interchange. This can be used as a proxy
    ///     to see if the RouteFromInterchange
    ///     and InterchangeStop will be null.
    /// </summary>
    public bool RequiresInterchange { get; set; }
    
    /// <summary>
    ///     Journey time minutes from the origin stop to
    ///     interchange stop or end of journey if no interchange is required.
    /// </summary>
    public int MinutesFromOrigin { get; init; }

    /// <summary>
    ///     Journey time minutes from the interchange stop to the
    ///     destination stop. This is 0 if there is no interchange.
    /// </summary>
    public int MinutesFromInterchange { get; init; }

    /// <summary>
    ///     Total journey time from origin to destination in minutes.
    ///     If there is no interchange, this will be the same as the
    ///     MinutesFromOrigin
    /// </summary>
    public int TotalJourneyTimeMinutes { get; set; }
}