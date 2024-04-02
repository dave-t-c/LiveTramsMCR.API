
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Common.Data.DynamoDb;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace LiveTramsMCR.Tests.Helpers;

internal static class DynamoDbHelper
{
    internal static async Task CreateRecords<T>(IEnumerable<T> items)
    where T: IDynamoDbTable
    {
        var dynamoDbContext = TestHelper.GetService<IDynamoDBContext>();
        var dynamoDbClient = TestHelper.GetService<IAmazonDynamoDB>();
        var instance = (IDynamoDbTable)Activator.CreateInstance(typeof(T));
        var createTableRequest = instance?.BuildCreateTableRequest();
        var existingTables = await dynamoDbClient.ListTablesAsync();

        if (!existingTables.TableNames.Contains(createTableRequest.TableName))
        {
            await dynamoDbClient.CreateTableAsync(createTableRequest);
        }
        
        var itemBatch = dynamoDbContext.CreateBatchWrite<T>();
        itemBatch.AddPutItems(items);
        await itemBatch.ExecuteAsync();
    }

    internal static async Task TearDownDatabase()
    {
        var dynamoDbClient = TestHelper.GetService<IAmazonDynamoDB>();
        var existingTables = await dynamoDbClient.ListTablesAsync();
        foreach (var tableName in existingTables.TableNames)
        {
            await dynamoDbClient.DeleteTableAsync(tableName);
        }
    }
}