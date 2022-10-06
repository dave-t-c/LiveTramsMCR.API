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
    /// Maps the route name to the destination of the Tram to the Interchange Stop or the Destination Stop
    /// E.g.Green line towards Bury (With the Stop value for Bury)
    /// </summary>
    public Dictionary<string, Stop> DestinationsFromOrigin { get; set; }
    
    /// <summary>
    /// Route from the Interchange stop to the
    /// destination stop. This may be null.
    /// </summary>
    public List<Route> RoutesFromInterchange { get; set; }
    
    /// <summary>
    /// Maps the route name to the destination of the Tram from the interchange stop.
    /// E.g.Green line towards Bury (With the Stop value for Bury)
    /// </summary>
    public Dictionary<string, Stop> DestinationsFromInterchange { get; set; }
    
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
}