using System.Collections.Generic;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Tests.Configuration;
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
}