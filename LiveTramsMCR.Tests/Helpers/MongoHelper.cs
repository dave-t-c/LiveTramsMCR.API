using System.Collections.Generic;
using LiveTramsMCR.Configuration;
using MongoDB.Driver;

namespace LiveTramsMCR.Tests.Helpers;

internal static class MongoHelper
{
    internal static void CreateRecords<T>(
        string collectionName,
        IEnumerable<T> items,
        string dbName = AppConfiguration.DatabaseName)
    {
        var mongoClient = TestHelper.GetService<MongoClient>();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<T>(collectionName);
        collection.InsertMany(items);
    }

    internal static void TearDownDatabase(
        string dbName = AppConfiguration.DatabaseName)
    {
        var mongoClient = TestHelper.GetService<MongoClient>();
        mongoClient.DropDatabase(dbName);
    }
}