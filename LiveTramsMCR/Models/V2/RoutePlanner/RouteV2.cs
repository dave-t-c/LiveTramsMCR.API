using System.Collections.Generic;
using System.Text.Json.Serialization;
using LiveTramsMCR.Models.V2.Stops;
using MongoDB.Bson;

namespace LiveTramsMCR.Models.V2.RoutePlanner;

/// <summary>
/// Represents route information. 
/// </summary>
public class RouteV2 {

    /// <summary>
    /// Object ID used by mongodb
    /// </summary>
    [JsonIgnore] 
    public ObjectId Id { get; set; }

    /// <summary>
    /// Name of the route, e.g. "Purple"
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Hex colour string for the route, e.g. #7B2082
    /// </summary>
    public string Colour { get; set; }

    /// <summary>
    /// Stops belonging to a route in the order they can be travelled between.
    /// </summary>
    public List<StopKeysV2> Stops { get; set; }
    
    #nullable enable
    /// <summary>
    /// Stop detail generated using the Stop keys
    /// Contains the full detail for all stops on the route.
    /// </summary>
    public List<StopV2>? StopsDetail { get; set; }
    #nullable disable

    /// <summary>
    /// List of co-ordinates to create a polyline for the route.
    /// This follows the same direction as the stops.
    /// </summary>
    public List<List<double>> PolylineCoordinates { get; set; }
}