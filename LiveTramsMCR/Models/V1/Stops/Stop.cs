using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using LiveTramsMCR.Common.Data.DynamoDb;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync.SynchronizationTasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

// ReSharper disable NonReadonlyMemberInGetHashCode
#pragma warning disable CS0618 // Member is obsolete

namespace LiveTramsMCR.Models.V1.Stops;

/// <summary>
///     Stores information about a single stop.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
[BsonIgnoreExtraElements]
[DynamoDBTable(AppConfiguration.StopsCollectionName)]
public sealed class Stop : IEquatable<Stop>, IDynamoDbTable, ISynchronizationType<Stop>
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
    ///     IDs associated with the stop. Larger stops will have more IDs.
    /// </summary>
    [Obsolete("A list of IDs for each stop is no longer maintained. Data will likely be out of date")]
    [DynamoDBProperty]
    public List<int> Ids { get; set; }

    /// <summary>
    ///     Naptan ID for the stop. This can be used to look up more information
    ///     in government transport data sets
    /// </summary>
    [DynamoDBProperty]
    public string AtcoCode { get; set; }

    /// <summary>
    ///     Stop Latitude. This may be different to that shown by apple or google maps.
    /// </summary>
    [DynamoDBProperty]
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
    ///     Checks equality of stops by checking name or tlaref
    /// </summary>
    /// <param name="other">Stop to compare</param>
    /// <returns></returns>
    public bool Equals(Stop other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StopName == other.StopName
               || Tlaref == other.Tlaref;

    }

    public bool CompareSyncData(Stop otherData)
    {
        return this.StopName == otherData.StopName;
    }

    public FilterDefinition<Stop> BuildFilter()
    {
        throw new NotImplementedException();
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
        return obj.GetType() == GetType() && Equals((Stop)obj);
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
            TableName = AppConfiguration.StopsCollectionName,
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
}