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
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using MongoDB.Driver;
using NUnit.Framework;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace LiveTramsMCR.Tests.TestDataSync;

public class TestRouteTimesSynchronization : BaseNunitTest
{
    private MongoClient? _mongoClient;
    private IRouteRepository? _routeRepository;
    private List<RouteTimes>? _routeTimes;
    private IMongoCollection<RouteTimes>? _routeTimesCollection;
    private SynchronizationTask<RouteTimes>? _synchronizationTask;
    
    [SetUp]
    public void SetUp()
    {
        _mongoClient = TestHelper.GetService<MongoClient>();
        var dynamoDbClient = TestHelper.GetService<IAmazonDynamoDB>();
        var dynamoDbContext = TestHelper.GetService<IDynamoDBContext>();
        _routeRepository = TestHelper.GetService<IRouteRepository>();
        var routeTimesPath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.RouteTimesPath);
        _routeTimes = FileHelper.ImportFromJsonFile<List<RouteTimes>>(routeTimesPath);
        var db = _mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        _routeTimesCollection = db.GetCollection<RouteTimes>(AppConfiguration.RouteTimesCollectionName);
        _synchronizationTask = new SynchronizationTask<RouteTimes>(_routeTimesCollection, dynamoDbClient, dynamoDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _mongoClient = null;
        _routeRepository = null;
        _routeTimes = null;
        _synchronizationTask = null;
    }

    [Test]
    public async Task TestCreateRoutesFromEmptyDb()
    {
        await _synchronizationTask.SyncData(_routeTimes);

        var createdRouteTimes = _routeRepository.GetAllRouteTimes();
        Assert.AreEqual(_routeTimes.Count, createdRouteTimes.Count);
    }

    [Test]
    public async Task TestUpdateExistingRoute()
    {
        await _routeTimesCollection.InsertManyAsync(_routeTimes);

        var purpleRoute = _routeTimes.First(route => route.Route == "Purple");

        purpleRoute.Times["Test stop name"] = "00:00:00";
        var purpleRouteIndex = _routeTimes.FindIndex(route => route.Route == "Purple");
        _routeTimes[purpleRouteIndex] = purpleRoute;

        await _synchronizationTask.SyncData(_routeTimes);
        
        var updatedRouteTimes = _routeRepository.GetAllRouteTimes();
        Assert.AreEqual(_routeTimes.Count, updatedRouteTimes.Count);

        var updatedPurpleRoute = updatedRouteTimes.First(route => route.Route == "Purple");
        Assert.AreEqual("00:00:00", updatedPurpleRoute.Times["Test stop name"]);
    }

    [Test]
    public async Task TestDeleteExistingRoute()
    {
        await _routeTimesCollection.InsertManyAsync(_routeTimes);
        
        _routeTimes.RemoveAll(route => route.Route == "Purple");

        await _synchronizationTask.SyncData(_routeTimes);
        
        var updatedRouteTimes = _routeRepository.GetAllRouteTimes();
        Assert.AreEqual(_routeTimes.Count, updatedRouteTimes.Count);
        Assert.IsNull(updatedRouteTimes.FirstOrDefault(route => route.Route == "Purple"));
    }

    [Test]
    public async Task TestCreateUpdateDelete()
    {
        await _routeTimesCollection.InsertManyAsync(_routeTimes);
        
        var purpleRoute = _routeTimes.First(route => route.Route == "Purple");
        purpleRoute.Times["Test stop name"] = "00:00:00";
        var purpleRouteIndex = _routeTimes.FindIndex(route => route.Route == "Purple");
        _routeTimes[purpleRouteIndex] = purpleRoute;
        
        _routeTimes.RemoveAll(route => route.Route == "Yellow");

        var createdRouteTimes = new RouteTimes
        {
            Route = "TestRoute",
            Times = new Dictionary<string, string>()
        };
        
        _routeTimes.Add(createdRouteTimes);

        await _synchronizationTask.SyncData(_routeTimes);
        
        var updatedRoutes = _routeRepository.GetAllRouteTimes();
        Assert.AreEqual(_routeTimes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Route == "Purple");
        Assert.AreEqual("00:00:00", updatedPurpleRoute.Times["Test stop name"]);
        
        Assert.IsNull(updatedRoutes.FirstOrDefault(route => route.Route == "Yellow"));

        var createdRouteResponse = updatedRoutes.FirstOrDefault(route => route.Route == "TestRoute");
        Assert.IsNotNull(createdRouteResponse);
    }
}