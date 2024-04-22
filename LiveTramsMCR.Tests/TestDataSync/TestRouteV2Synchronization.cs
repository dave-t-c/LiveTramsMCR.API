using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync.Helpers;
using LiveTramsMCR.DataSync.SynchronizationTasks;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using MongoDB.Driver;
using NUnit.Framework;
using DynamoDbHelper = LiveTramsMCR.Common.Data.DynamoDb.DynamoDbHelper;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace LiveTramsMCR.Tests.TestDataSync;

public class TestRouteV2Synchronization : BaseNunitTest
{
    private SynchronizationTask<RouteV2>? _routeSynchronization;
    private MongoClient? _mongoClient;
    private IRouteRepositoryV2? _routeRepository;
    private List<RouteV2>? _routes;
    private IMongoCollection<RouteV2>? _routeCollection;
    
    [SetUp]
    public async Task SetUp()
    {
        _mongoClient = TestHelper.GetService<MongoClient>();
        var dynamoDbClient = TestHelper.GetService<IAmazonDynamoDB>();
        var dynamoDbContext = TestHelper.GetService<IDynamoDBContext>();
        _routeRepository = TestHelper.GetService<IRouteRepositoryV2>();
        var routePath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.RoutesV2Path);
        _routes = FileHelper.ImportFromJsonFile<List<RouteV2>>(routePath);
        var db = _mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        _routeCollection = db.GetCollection<RouteV2>(AppConfiguration.RoutesV2CollectionName);
        _routeSynchronization = new SynchronizationTask<RouteV2>(_routeCollection, dynamoDbClient, dynamoDbContext);
        
        // Populate stops as routes are dependent on stopsV2
        var stopsV2Collection = db.GetCollection<StopV2>(AppConfiguration.StopsV2CollectionName);
        var stopsPath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.StopsV2Path);
        var stops = FileHelper.ImportFromJsonFile<List<StopV2>>(stopsPath);
        await stopsV2Collection.InsertManyAsync(stops);
        await DynamoDbTestHelper.CreateRecords(stops);
    }

    [TearDown]
    public void TearDown()
    {
        _routeSynchronization = null;
        _mongoClient = null;
        _routeRepository = null;
        _routes = null;
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, null);
    }

    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestCreateRoutesFromEmptyDb(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _routeSynchronization.SyncData(_routes);

        var createdRoutes = _routeRepository.GetRoutes();
        Assert.AreEqual(_routes.Count, createdRoutes.Count);
    }
    
    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestUpdateExistingRoute(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _routeCollection.InsertManyAsync(_routes);
        await DynamoDbTestHelper.CreateRecords(_routes);

        var purpleRoute = _routes.First(route => route.Name == "Purple");

        purpleRoute.PolylineCoordinates.Add(new List<double>() {1.0, 2.0});
        var purpleRouteIndex = _routes.FindIndex(route => route.Name == "Purple");
        _routes[purpleRouteIndex] = purpleRoute;

        await _routeSynchronization.SyncData(_routes);
        
        var updatedRoutes = _routeRepository.GetRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Name == "Purple");
        Assert.AreEqual(purpleRoute.PolylineCoordinates.Count, updatedPurpleRoute.PolylineCoordinates.Count);
    }

    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestDeleteExistingRoute(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _routeCollection.InsertManyAsync(_routes);
        await DynamoDbTestHelper.CreateRecords(_routes);
        
        _routes.RemoveAll(route => route.Name == "Purple");

        await _routeSynchronization.SyncData(_routes);
        
        var updatedRoutes = _routeRepository.GetRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);
        Assert.IsNull(updatedRoutes.FirstOrDefault(route => route.Name == "Purple"));
    }

    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestCreateUpdateDelete(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _routeCollection.InsertManyAsync(_routes);
        await DynamoDbTestHelper.CreateRecords(_routes);
        
        var purpleRoute = _routes.First(route => route.Name == "Purple");
        purpleRoute.Colour = "Updated route colour";
        var purpleRouteIndex = _routes.FindIndex(route => route.Name == "Purple");
        _routes[purpleRouteIndex] = purpleRoute;
        
        _routes.RemoveAll(route => route.Name == "Yellow");

        var createdRoute = new RouteV2
        {
            Name = "Test Route", 
            Colour = "Test",
            Stops = new List<StopKeysV2>()
        };
        
        _routes.Add(createdRoute);

        await _routeSynchronization.SyncData(_routes);
        
        var updatedRoutes = _routeRepository.GetRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Name == "Purple");
        Assert.AreEqual("Updated route colour", updatedPurpleRoute.Colour);
        
        Assert.IsNull(updatedRoutes.FirstOrDefault(route => route.Name == "Yellow"));

        var createdRouteResponse = updatedRoutes.FirstOrDefault(route => route.Name == "Test Route");
        Assert.IsNotNull(createdRouteResponse);
    }
}