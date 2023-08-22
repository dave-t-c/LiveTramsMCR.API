using System.Collections.Generic;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;

/// <summary>
/// Request model for next service identifier v2.
/// </summary>
public class NextServiceIdentifierV2Request
{
    /// <summary>
    /// Origin of the journey
    /// </summary>
    public StopV2 Origin { get; init; }
    
    /// <summary>
    /// Destination or interchange of the journey.
    /// This should be on the same route as the origin stop.
    /// </summary>
    public StopV2 Destination { get; init; }
    
    /// <summary>
    /// Trams from a live service request.
    /// </summary>
    public List<Tram> Services { get; init; }
    
    /// <summary>
    /// Routes between the origin and interchange or destination.
    /// </summary>
    public List<RouteV2> Routes { get; init; }
}