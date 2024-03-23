using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LiveTramsMCR.Models.V2.Stops;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Routes;

/// <summary>
///     Class that represents a tram route between two Stops.
///     The stops are included as the Stop class, so all relevant information is available.
/// </summary>
[BsonIgnoreExtraElements]
public class SimpleRouteV2
{

    /// <summary>
    ///     Creates a new route, the params are only null sanitised
    /// </summary>
    /// <param name="name">Route name, e.g. Purple</param>
    /// <param name="colour">Route hex colour, e.g. #7B2082</param>
    /// <param name="stops"></param>
    public SimpleRouteV2(string name, string colour, List<StopKeysV2> stops)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Colour = colour ?? throw new ArgumentNullException(nameof(colour));
        Stops = stops ?? throw new ArgumentNullException(nameof(stops));
    }
    /// <summary>
    ///     Object ID used by mongodb
    /// </summary>
    [JsonIgnore]
    public ObjectId Id { get; set; }

    /// <summary>
    ///     Name of the route, e.g. "Purple"
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    ///     Hex colour string for the route, e.g. #7B2082
    /// </summary>
    public string Colour { get; internal set; }

    /// <summary>
    ///     Stops belonging to a route in the order they can be travelled between.
    /// </summary>
    public List<StopKeysV2> Stops { get; internal set; }
}