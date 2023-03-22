using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
/// Maps a route name, e.g. Purple to a dictionary of it's stops
/// against an example timetable entry.
/// </summary>
public class RouteTimes
{
    /// <summary>
    /// Object ID field used in cosmos-db.
    /// This is not used internally
    /// </summary>
    [JsonIgnore]
    [JsonPropertyName("_id")]
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// Name of the route
    /// </summary>
    public String Route { get; set; }
    
    /// <summary>
    /// Times for each stop on the route
    /// </summary>
    public Dictionary<String, TimeSpan> Times { get; set; }
}