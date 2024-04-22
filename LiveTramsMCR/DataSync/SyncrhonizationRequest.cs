using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using MongoDB.Driver;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace LiveTramsMCR.DataSync;

public class SynchronizationRequest
{
    public IMongoClient MongoClient { get; set; }
    
    public IAmazonDynamoDB DynamoDbClient { get; set; }
    
    public IDynamoDBContext DynamoDbContext { get; set; }
    
    public string TargetDbName { get; set; }
    
    public string StopsCollectionName { get; set; }
    
    public string StopsPath { get; set; }
    
    public string StopsV2CollectionName { get; set; }
    
    public string StopsV2Path { get; set; }
    
    public string RouteTimesCollectionName { get; set; }
    
    public string RouteTimesPath { get; set; }
    
    public string RoutesCollectionName { get; set; }
    
    public string RoutesPath { get; set; }
    
    public string RoutesV2CollectionName { get; set; }
    
    public string RoutesV2Path { get; set; }
}