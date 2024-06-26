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
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using MongoDB.Driver;
using NUnit.Framework;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace LiveTramsMCR.Tests.TestDataSync;

public class TestRouteSynchronization : BaseNunitTest
{
    private MongoClient? _mongoClient;
    private IRouteRepository? _routeRepository;
    private List<Route>? _routes;
    private IMongoCollection<Route>? _routeCollection;
    private SynchronizationTask<Route>? _synchronizationTask;
    
    [SetUp]
    public void SetUp()
    {
        _mongoClient = TestHelper.GetService<MongoClient>();
        var dynamoDbClient = TestHelper.GetService<IAmazonDynamoDB>();
        var dynamodbContext = TestHelper.GetService<IDynamoDBContext>();
        _routeRepository = TestHelper.GetService<IRouteRepository>();
        var routePath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.RoutesPath);
        _routes = FileHelper.ImportFromJsonFile<List<Route>>(routePath);
        var db = _mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        _routeCollection = db.GetCollection<Route>(AppConfiguration.RoutesCollectionName);
        _synchronizationTask = new SynchronizationTask<Route>(_routeCollection, dynamoDbClient, dynamodbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _mongoClient = null;
        _routeRepository = null;
        _routes = null;
        _synchronizationTask = null;
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, null);
    }
    
    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestCreateRoutesFromEmptyDb(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _synchronizationTask.SyncData(_routes);

        var createdRoutes = _routeRepository.GetAllRoutes();
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
        purpleRoute.Stops.Add(new Stop());
        var purpleRouteIndex = _routes.FindIndex(route => route.Name == "Purple");
        _routes[purpleRouteIndex] = purpleRoute;

        await _synchronizationTask.SyncData( _routes);
        
        var updatedRoutes = _routeRepository.GetAllRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Name == "Purple");
        Assert.AreEqual(purpleRoute.Stops.Count, updatedPurpleRoute.Stops.Count);
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

        await _synchronizationTask.SyncData(_routes);
        
        var updatedRoutes = _routeRepository.GetAllRoutes();
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
        purpleRoute.Stops.Add(new Stop());
        var purpleRouteIndex = _routes.FindIndex(route => route.Name == "Purple");
        _routes[purpleRouteIndex] = purpleRoute;
        
        _routes.RemoveAll(route => route.Name == "Yellow");

        var createdRoute = new Route("Test Route", "Test", new List<Stop>());
        
        _routes.Add(createdRoute);

        await _synchronizationTask.SyncData(_routes);
        
        var updatedRoutes = _routeRepository.GetAllRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Name == "Purple");
        Assert.AreEqual(purpleRoute.Stops.Count, updatedPurpleRoute.Stops.Count);
        
        Assert.IsNull(updatedRoutes.FirstOrDefault(route => route.Name == "Yellow"));

        var createdRouteResponse = updatedRoutes.FirstOrDefault(route => route.Name == "Test Route");
        Assert.IsNotNull(createdRouteResponse);
    }
    
    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestCreateUpdateDeleteDynamoDb(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _routeCollection.InsertManyAsync(_routes);
        await DynamoDbTestHelper.CreateRecords(_routes);
        
        var purpleRoute = _routes.First(route => route.Name == "Purple");
        purpleRoute.Stops.Add(new Stop());
        var purpleRouteIndex = _routes.FindIndex(route => route.Name == "Purple");
        _routes[purpleRouteIndex] = purpleRoute;
        
        _routes.RemoveAll(route => route.Name == "Yellow");

        var createdRoute = new Route("Test Route", "Test", new List<Stop>());
        
        _routes.Add(createdRoute);

        await _synchronizationTask.SyncData(_routes);
        
        var updatedRoutes = _routeRepository.GetAllRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Name == "Purple");
        Assert.AreEqual(purpleRoute.Stops.Count, updatedPurpleRoute.Stops.Count);
        
        Assert.IsNull(updatedRoutes.FirstOrDefault(route => route.Name == "Yellow"));

        var createdRouteResponse = updatedRoutes.FirstOrDefault(route => route.Name == "Test Route");
        Assert.IsNotNull(createdRouteResponse);
    }
}