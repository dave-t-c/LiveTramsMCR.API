using System.Collections.Generic;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.RoutePlanner;

/// <summary>
/// POCO object that details a planned route between an origin and destination stop.
/// </summary>
public class PlannedJourney
{
    /// <summary>
    /// Stop the route originates / starts at.
    /// </summary>
    public Stop OriginStop { get; set; }
    
    /// <summary>
    /// End stop for the route
    /// </summary>
    public Stop DestinationStop { get; set; }
    
    /// <summary>
    /// Stop where an interchange needs to be made to
    /// continue on the route. This may be null.
    /// </summary>
    public Stop InterchangeStop { get; set; }
    
    /// <summary>
    /// Routes taken from the origin destination
    /// to either the interchange stop or destination
    /// stop if no interchange is required
    /// (Only require a single route).
    /// </summary>
    public List<Route> RoutesFromOrigin { get; set; }
    
    /// <summary>
    /// Stops between the origin stop and interchange or destination for each route.
    /// </summary>
    public List<Stop> StopsFromOrigin { get; set; }
    
    /// <summary>
    /// Maps the route name to the destination of the Tram to the Interchange Stop or the Destination Stop
    /// E.g.Green line towards Bury (With the Stop value for Bury)
    /// </summary>
    public HashSet<Stop> TerminiFromOrigin { get; set; }

    /// <summary>
    /// Route from the Interchange stop to the
    /// destination stop. This may be null.
    /// </summary>
    public List<Route> RoutesFromInterchange { get; set; }
    
    /// <summary>
    /// Stops between the interchange and destination stop.
    /// </summary>
    public List<Stop> StopsFromInterchange { get; set; }
    
    /// <summary>
    /// Maps the route name to the destination of the Tram from the interchange stop.
    /// E.g.Green line towards Bury (With the Stop value for Bury)
    /// </summary>
    public HashSet<Stop> TerminiFromInterchange { get; set; }
    
    
    /// <summary>
    /// Boolean showing if the route requires an
    /// interchange. This can be used as a proxy
    /// to see if the RouteFromInterchange
    /// and InterchangeStop will be null.
    /// </summary>
    public bool RequiresInterchange { get; set; }
    
    /// <summary>
    /// Useful information about the route
    /// not covered by the other fields.
    /// </summary>
    public string RouteDetails { get; set; }
    
    /// <summary>
    /// Journey time minutes from the origin stop to
    /// interchange stop or end of journey if no interchange is required.
    /// </summary>
    public int MinutesFromOrigin { get; init; }
    
    /// <summary>
    /// Journey time minutes from the interchange stop to the
    /// destination stop. This is 0 if there is no interchange.
    /// </summary>
    public int MinutesFromInterchange { get; init; }
    
    /// <summary>
    /// Total journey time from origin to destination in minutes.
    /// If there is no interchange, this will be the same as the
    /// MinutesFromOrigin
    /// </summary>
    public int TotalJourneyTimeMinutes { get; set; }
}