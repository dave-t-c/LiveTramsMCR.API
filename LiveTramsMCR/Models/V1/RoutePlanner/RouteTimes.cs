using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
///     Maps a route name, e.g. Purple to a dictionary of it's stops
///     against an example timetable entry.
/// </summary>
[BsonIgnoreExtraElements]
public class RouteTimes
{
    /// <summary>
    ///     Name of the route
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    ///     Times for each stop on the route
    /// </summary>
    public Dictionary<string, TimeSpan> Times { get; set; }
}