using System.Collections.Generic;

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
    public List<List<double>> PolylineFromOrigin { get; init; }
    
    /// <summary>
    /// Polyline from interchange to destination
    /// Will be null if no interchange is required
    /// </summary>
    public List<List<double>> PolylineFromInterchange { get; init; }
}