using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using LiveTramsMCR.Common.Data.DynamoDb;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync.SynchronizationTasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
///     Maps a route name, e.g. Purple to a dictionary of it's stops
///     against an example timetable entry.
/// </summary>
[BsonIgnoreExtraElements]
[DynamoDBTable(AppConfiguration.RouteTimesCollectionName)]
public class RouteTimes : IDynamoDbTable, ISynchronizationType<RouteTimes>
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

    /// <inheritdoc />
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
            BillingMode = BillingMode.PAY_PER_REQUEST
        };
    }

    /// <inheritdoc />
    public bool CompareSyncData(RouteTimes otherData)
    {
        return this.Route != otherData.Route;
    }

    /// <inheritdoc />
    public FilterDefinition<RouteTimes> BuildFilter()
    {
        var filter = Builders<RouteTimes>.Filter
            .Eq(routeTimes => routeTimes.Route, this.Route);
        return filter;
    }
}