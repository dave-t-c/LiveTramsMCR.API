using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using LiveTramsMCR.Common.Data.DynamoDb;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync.SynchronizationTasks;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
///     Stores information about a single stop.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
[BsonIgnoreExtraElements]
[DynamoDBTable(AppConfiguration.StopsV2CollectionName)]
public sealed class StopV2 : IEquatable<StopV2>, IEqualityComparer<StopV2>, IDynamoDbTable, ISynchronizationType<StopV2>
{
    /// <summary>
    ///     Name of the stop, such as Piccadilly
    /// </summary>
    [DynamoDBRangeKey]
    public string StopName { get; set; }

    /// <summary>
    ///     3 code ID for the stop, e.g. PIC for Piccadilly
    /// </summary>
    [DynamoDBHashKey]
    public string Tlaref { get; set; }

    /// <summary>
    ///     Routes the stop is on.
    /// </summary>
    [DynamoDBProperty]
    public List<SimpleRouteV2> Routes { get; set; }

    /// <summary>
    ///     Naptan ID for the stop. This can be used to look up more information
    ///     in government transport data sets
    /// </summary>
    [DynamoDBProperty]
    public string AtcoCode { get; set; }

    /// <summary>
    ///     Stop Latitude. This may be different to that shown by apple or google maps.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    ///     Stop Longitude. This may be different to that shown by apple or google maps.
    /// </summary>
    [DynamoDBProperty]
    public double Longitude { get; set; }

    /// <summary>
    ///     Street the stop is on. If it is not directly on a street, it will be prefixed
    ///     with 'Off'.
    /// </summary>
    [DynamoDBProperty]
    public string Street { get; set; }

    /// <summary>
    ///     Closest road intersection to the stop. For stops where there is not a close intersection,
    ///     this will be blank.
    /// </summary>
    [DynamoDBProperty]
    public string RoadCrossing { get; set; }

    /// <summary>
    ///     Line the stop is on. This is a single value and does not contain all lines.
    ///     This will be a destination, such as Bury, and does not include the line colour(s).
    /// </summary>
    [DynamoDBProperty]
    public string Line { get; set; }

    /// <summary>
    ///     Ticket fare zone for the stop. If a stop is in multiple zones, it will
    ///     be shown as 'a/b', where a is the smaller of the two zones, e.g. '3/4'.
    /// </summary>
    [DynamoDBProperty]
    public string StopZone { get; set; }

    /// <summary>
    ///     Compares two stop objects
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(StopV2 x, StopV2 y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.StopName == y.StopName && x.Tlaref == y.Tlaref;
    }

    /// <summary>
    ///     Checks equality of stops by checking name or tlaref
    /// </summary>
    /// <param name="other">Stop to compare</param>
    /// <returns></returns>
    public bool Equals(StopV2 other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StopName == other.StopName
               || Tlaref == other.Tlaref;

    }

    public bool CompareSyncData(StopV2 otherData)
    {
        return this.StopName != otherData.StopName;
    }

    public FilterDefinition<StopV2> BuildFilter()
    {
        var filter = Builders<StopV2>.Filter
            .Eq(stop => stop.Tlaref, this.Tlaref);
        return filter;
    }

    /// <summary>
    ///     Checks equality between this stop and another obj
    /// </summary>
    /// <param name="obj">Obj to compare</param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((StopV2)obj);
    }

    /// <summary>
    ///     Generates a hash code for a stop
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(StopName);
        return hashCode.ToHashCode();
    }

    public CreateTableRequest BuildCreateTableRequest()
    {
        return new CreateTableRequest()
        {
            TableName = AppConfiguration.StopsV2CollectionName,
            KeySchema = new List<KeySchemaElement>
            {
                new()
                {
                    AttributeName = nameof(Tlaref),
                    KeyType = KeyType.HASH
                },
                new()
                {
                    AttributeName = nameof(StopName),
                    KeyType = KeyType.RANGE
                }
            },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new()
                {
                    AttributeName = nameof(Tlaref),
                    AttributeType = ScalarAttributeType.S
                },
                new()
                {
                    AttributeName = nameof(StopName),
                    AttributeType = ScalarAttributeType.S
                }
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = AppConfiguration.DefaultReadCapacityUnits,
                WriteCapacityUnits = AppConfiguration.DefaultWriteCapacityUnits
            }
        };
    }


    /// <summary>
    ///     Generates a hash code for a stop obj
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(StopV2 obj)
    {
        return HashCode.Combine(obj.StopName, obj.Tlaref);
    }
}