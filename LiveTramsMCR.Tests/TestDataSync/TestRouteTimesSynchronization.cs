using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    private RouteTimesSynchronization? _routeTimesSynchronization;
    private MongoClient? _mongoClient;
    private IRouteRepository? _routeRepository;
    private List<RouteTimes>? _routeTimes;
    private IMongoCollection<RouteTimes>? _routeTimesCollection;
    
    [SetUp]
    public void SetUp()
    {
        _routeTimesSynchronization = new RouteTimesSynchronization();
        _mongoClient = TestHelper.GetService<MongoClient>();
        _routeRepository = TestHelper.GetService<IRouteRepository>();
        var routeTimesPath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.RouteTimesPath);
        _routeTimes = FileHelper.ImportFromJsonFile<List<RouteTimes>>(routeTimesPath);
        var db = _mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        _routeTimesCollection = db.GetCollection<RouteTimes>(AppConfiguration.RouteTimesCollectionName);
    }

    [TearDown]
    public void TearDown()
    {
        _routeTimesSynchronization = null;
        _mongoClient = null;
        _routeRepository = null;
        _routeTimes = null;
    }

    [Test]
    public async Task TestCreateRoutesFromEmptyDb()
    {
        await _routeTimesSynchronization.SyncData(_routeTimesCollection, _routeTimes);

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

        await _routeTimesSynchronization.SyncData(_routeTimesCollection, _routeTimes);
        
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

        await _routeTimesSynchronization.SyncData(_routeTimesCollection, _routeTimes);
        
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

        await _routeTimesSynchronization.SyncData(_routeTimesCollection, _routeTimes);
        
        var updatedRoutes = _routeRepository.GetAllRouteTimes();
        Assert.AreEqual(_routeTimes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Route == "Purple");
        Assert.AreEqual("00:00:00", updatedPurpleRoute.Times["Test stop name"]);
        
        Assert.IsNull(updatedRoutes.FirstOrDefault(route => route.Route == "Yellow"));

        var createdRouteResponse = updatedRoutes.FirstOrDefault(route => route.Route == "TestRoute");
        Assert.IsNotNull(createdRouteResponse);
    }
}