using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using LiveTramsMCR.Common.Data.DynamoDb;
using LiveTramsMCR.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
///     Maps a route name, e.g. Purple to a dictionary of it's stops
///     against an example timetable entry.
/// </summary>
[BsonIgnoreExtraElements]
[DynamoDBTable(AppConfiguration.RouteTimesCollectionName)]
public class RouteTimes : IDynamoDbTable
{
    /// <summary>
    ///     Name of the route
    /// </summary>
    [DynamoDBHashKey]
    public string Route { get; set; }

    /// <summary>
    ///     Times for each stop on the route
    /// </summary>
    [DynamoDBProperty]
    public Dictionary<string, string> Times { get; set; }

    public CreateTableRequest BuildCreateTableRequest()
    {
        return new CreateTableRequest()
        {
            TableName = AppConfiguration.RouteTimesCollectionName,
            KeySchema = new List<KeySchemaElement>
            {
                new()
                {
                    AttributeName = nameof(Route),
                    KeyType = KeyType.HASH
                }
            },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new()
                {
                    AttributeName = nameof(Route),
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