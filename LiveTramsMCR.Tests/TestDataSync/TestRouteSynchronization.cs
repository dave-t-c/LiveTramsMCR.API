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
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using MongoDB.Driver;
using NUnit.Framework;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace LiveTramsMCR.Tests.TestDataSync;

public class TestRouteSynchronization : BaseNunitTest
{
    private RouteSynchronization? _routeSynchronization;
    private MongoClient? _mongoClient;
    private IRouteRepository? _routeRepository;
    private List<Route>? _routes;
    private IMongoCollection<Route>? _routeCollection;
    
    [SetUp]
    public void SetUp()
    {
        _routeSynchronization = new RouteSynchronization();
        _mongoClient = TestHelper.GetService<MongoClient>();
        _routeRepository = TestHelper.GetService<IRouteRepository>();
        var routePath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.RoutesPath);
        _routes = FileHelper.ImportFromJsonFile<List<Route>>(routePath);
        var db = _mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        _routeCollection = db.GetCollection<Route>(AppConfiguration.RoutesCollectionName);

    }

    [TearDown]
    public void TearDown()
    {
        _routeSynchronization = null;
        _mongoClient = null;
        _routeRepository = null;
        _routes = null;
    }

    /// <summary>
    /// Test to create stops from an empty DB.
    /// All 99 stops should be created.
    /// </summary>
    [Test]
    public async Task TestCreateRoutesFromEmptyDb()
    {
        await _routeSynchronization.SyncData(_routeCollection, _routes);

        var createdRoutes = _routeRepository.GetAllRoutes();
        Assert.AreEqual(_routes.Count, createdRoutes.Count);
    }

    /// <summary>
    /// Test to update an existing stop wth a new value.
    /// </summary>
    [Test]
    public async Task TestUpdateExistingRoute()
    {
        await _routeCollection.InsertManyAsync(_routes);

        var purpleRoute = _routes.First(route => route.Name == "Purple");

        purpleRoute.Colour = "Updated route colour";
        var purpleRouteIndex = _routes.FindIndex(route => route.Name == "Purple");
        _routes[purpleRouteIndex] = purpleRoute;

        await _routeSynchronization.SyncData(_routeCollection, _routes);
        
        var updatedRoutes = _routeRepository.GetAllRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Name == "Purple");
        Assert.AreEqual("Updated route colour", updatedPurpleRoute.Colour);
    }

    [Test]
    public async Task TestDeleteExistingRoute()
    {
        await _routeCollection.InsertManyAsync(_routes);
        
        _routes.RemoveAll(route => route.Name == "Purple");

        await _routeSynchronization.SyncData(_routeCollection, _routes);
        
        var updatedRoutes = _routeRepository.GetAllRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);
        Assert.IsNull(updatedRoutes.FirstOrDefault(route => route.Name == "Purple"));
    }

    [Test]
    public async Task TestCreateUpdateDelete()
    {
        await _routeCollection.InsertManyAsync(_routes);
        
        var purpleRoute = _routes.First(route => route.Name == "Purple");
        purpleRoute.Colour = "Updated route colour";
        var purpleRouteIndex = _routes.FindIndex(route => route.Name == "Purple");
        _routes[purpleRouteIndex] = purpleRoute;
        
        _routes.RemoveAll(route => route.Name == "Yellow");

        var createdRoute = new Route("Test Route", "Test", new List<Stop>());
        
        _routes.Add(createdRoute);

        await _routeSynchronization.SyncData(_routeCollection, _routes);
        
        var updatedRoutes = _routeRepository.GetAllRoutes();
        Assert.AreEqual(_routes.Count, updatedRoutes.Count);

        var updatedPurpleRoute = updatedRoutes.First(route => route.Name == "Purple");
        Assert.AreEqual("Updated route colour", updatedPurpleRoute.Colour);
        
        Assert.IsNull(updatedRoutes.FirstOrDefault(route => route.Name == "Yellow"));

        var createdRouteResponse = updatedRoutes.FirstOrDefault(route => route.Name == "Test Route");
        Assert.IsNotNull(createdRouteResponse);
    }
}