using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Models.V2.Stops;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
// ReSharper disable ClassNeverInstantiated.Global

namespace LiveTramsMCR.Models.V2.RoutePlanner.Routes;

/// <summary>
///     Class that represents a tram route between two Stops.
///     The stops are included as the Stop class, so all relevant information is available.
/// </summary>
[BsonIgnoreExtraElements]
public class SimpleRouteV2
{
    /// <summary>
    ///     Name of the route, e.g. "Purple"
    /// </summary>
    [DynamoDBHashKey]
    public string Name { get; internal set; }

    /// <summary>
    ///     Hex colour string for the route, e.g. #7B2082
    /// </summary>
    [DynamoDBRangeKey]
    public string Colour { get; internal set; }

    /// <summary>
    ///     Stops belonging to a route in the order they can be travelled between.
    /// </summary>
    [DynamoDBProperty]
    public List<StopKeysV2> Stops { get; internal set; }
}