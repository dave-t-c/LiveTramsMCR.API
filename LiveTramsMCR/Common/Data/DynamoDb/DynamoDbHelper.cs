using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace LiveTramsMCR.Common.Data.DynamoDb;

/// <summary>
/// Utility class for working with DynamoDB
/// </summary>
public static class DynamoDbHelper
{
    /// <summary>
    /// Creates records in a dynamo db table.
    /// If the table does not exist it is created.
    /// </summary>
    public static async Task CreateRecords<T>(
        IDynamoDBContext dynamoDbContext,
        IAmazonDynamoDB dynamoDbClient,
        IEnumerable<T> items)
        where T : IDynamoDbTable
    {
        var tableExists = await TableExists(typeof(T), dynamoDbClient); 
        if (!tableExists)
        {
            var instance = (IDynamoDbTable)Activator.CreateInstance(typeof(T));
            var createTableRequest = instance?.BuildCreateTableRequest();
            await dynamoDbClient.CreateTableAsync(createTableRequest);
        }
            
        var itemBatch = dynamoDbContext.CreateBatchWrite<T>();
        itemBatch.AddPutItems(items);
        await itemBatch.ExecuteAsync();
    }

    /// <summary>
    /// Retrieves existing records of a given type.
    /// </summary>
    public static async Task<List<T>> RetrieveExistingRecords<T>(
        IDynamoDBContext dynamoDbContext,
        IAmazonDynamoDB dynamoDbClient)
        where T : IDynamoDbTable
    {
        var tableExists = await TableExists(typeof(T), dynamoDbClient);
        if (!tableExists)
        {
            return new List<T>();
        }
            
        var results = await dynamoDbContext.ScanAsync<T>(default).GetRemainingAsync();

        return results.ToList();
    }
    
    /// <summary>
    /// Retrieves the table name for a table
    /// </summary>
    public static string GetTableName(Type tableType)
    {
        var tableNameProperty = tableType
            .GetCustomAttributes<DynamoDBTableAttribute>()
            .SingleOrDefault();

        var tableName = tableNameProperty?.TableName;
        
        if (tableName is null)
        {
            throw new InvalidOperationException($"No DynamoDBTableAttribute Table name for type {tableType}");
        }

        return tableName;
    }

    /// <summary>
    /// Determines if a table exists for a type.
    /// </summary>
    public static async Task<bool> TableExists(
        Type tableType,
        IAmazonDynamoDB dynamoDbClient)
    {
        var existingTables = await dynamoDbClient.ListTablesAsync();
        var tableName = GetTableName(tableType);

        return existingTables.TableNames.Contains(tableName);
    }
}