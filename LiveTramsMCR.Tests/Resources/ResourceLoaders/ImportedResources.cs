using System.Collections.Generic;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V2.RoutePlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
///     Class for storing the resources that have been imported into the program.
/// </summary>
public class ImportedResources
{
    /// <summary>
    /// Creates a new imported resources with empty objects.
    /// </summary>
    public ImportedResources()
    {
        ImportedStops = new List<Stop>();
        ImportedStopsV2 = new List<StopV2>();
        StationNamesToTlaref = new Dictionary<string, string>();
        TlarefsToIds = new Dictionary<string, List<int>>();
        ImportedRoutes = new List<Route>();
        ImportedRouteTimes = new List<RouteTimes>();
        ImportedRoutesV2 = new List<RouteV2>();
    }

    /// <summary>
    /// Stores the stops imported into the application
    /// </summary>
    public List<Stop> ImportedStops { get; init; }
    
    /// <summary>
    /// Stores the stop v2s imported.
    /// </summary>
    public List<StopV2> ImportedStopsV2 { get; init; }
    
    /// <summary>
    /// Stores a dict from the station name to it's associated tlaref
    /// </summary>
    public Dictionary<string, string> StationNamesToTlaref { get; init; }
    
    /// <summary>
    /// Stores a dict from a stops tlaref, to a list of it's IDs. A stop can have multiple IDs.
    /// An ID here is usually associated with a passenger information display.
    /// </summary>
    public Dictionary<string, List<int>> TlarefsToIds { get; init; }
    
    /// <summary>
    /// Stores routes imported into the application, used in route planning.
    /// </summary>
    public List<Route> ImportedRoutes { get; init; }
    
    /// <summary>
    /// Stores the imported route times, used for calculating journey times.
    /// </summary>
    public List<RouteTimes> ImportedRouteTimes { get; init; }
    
    /// <summary>
    /// Stores the imported RouteV2s
    /// </summary>
    public List<RouteV2> ImportedRoutesV2 { get; init; }
}