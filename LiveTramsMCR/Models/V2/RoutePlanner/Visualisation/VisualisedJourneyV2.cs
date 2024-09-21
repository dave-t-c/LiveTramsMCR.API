using System.Collections.Generic;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;

/// <summary>
/// Model for data returned for journey visualisation, e.g. polylines. 
/// </summary>
public class VisualisedJourneyV2
{
    /// <summary>
    /// Polyline from origin to interchange,
    /// or end of journey if no interchange required
    /// </summary>
    public List<RouteV2.RouteCoordinate> PolylineFromOrigin { get; init; }
    
    /// <summary>
    /// Polyline from interchange to destination
    /// Will be null if no interchange is required
    /// </summary>
    public List<RouteV2.RouteCoordinate> PolylineFromInterchange { get; init; }
}