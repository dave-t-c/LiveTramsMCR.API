using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync;
using LiveTramsMCR.DataSync.Helpers;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using MongoDB.Driver;
using NUnit.Framework;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace LiveTramsMCR.Tests.TestDataSync;

public class TestSynchronizer : BaseNunitTest
{
    private MongoClient? _mongoClient;
    private IAmazonDynamoDB? _dynamoDbClient;
    private IDynamoDBContext? _dynamoDbContext;
    private Synchronizer? _synchronizer;
    private IRouteRepository? _routeRepository;
    private IRouteRepositoryV2? _routeRepositoryV2;
    private IStopsRepository? _stopsRepository;
    private IStopsRepositoryV2? _stopsRepositoryV2;
    private SynchronizationRequest? _request;
    private List<Stop>? _stops;
    private List<StopV2>? _stopV2S;
    private List<Route>? _routes;
    private List<RouteV2>? _routeV2S;
    private List<RouteTimes>? _routeTimes;
    
    [SetUp]
    public void SetUp()
    {
        _synchronizer = new Synchronizer();
        _mongoClient = TestHelper.GetService<MongoClient>();
        _dynamoDbClient = TestHelper.GetService<IAmazonDynamoDB>();
        _dynamoDbContext = TestHelper.GetService<IDynamoDBContext>();
        _routeRepository = TestHelper.GetService<IRouteRepository>();
        _routeRepositoryV2 = TestHelper.GetService<IRouteRepositoryV2>();
        _stopsRepository = TestHelper.GetService<IStopsRepository>();
        _stopsRepositoryV2 = TestHelper.GetService<IStopsRepositoryV2>();

        _request = new SynchronizationRequest
        {
            MongoClient = _mongoClient,
            DynamoDbContext = _dynamoDbContext,
            DynamoDbClient = _dynamoDbClient,
            TargetDbName = AppConfiguration.DatabaseName,
            StopsCollectionName = AppConfiguration.StopsCollectionName,
            StopsPath = AppConfiguration.StopsPath,
            StopsV2CollectionName = AppConfiguration.StopsV2CollectionName,
            StopsV2Path = AppConfiguration.StopsV2Path,
            RouteTimesCollectionName = AppConfiguration.RouteTimesCollectionName,
            RouteTimesPath = AppConfiguration.RouteTimesPath,
            RoutesCollectionName = AppConfiguration.RoutesCollectionName,
            RoutesPath = AppConfiguration.RoutesPath,
            RoutesV2CollectionName = AppConfiguration.RoutesV2CollectionName,
            RoutesV2Path = AppConfiguration.RoutesV2Path
        };

        _stops = FileHelper.ImportFromJsonFile<List<Stop>>(AppConfiguration.StopsPath);
        _stopV2S = FileHelper.ImportFromJsonFile<List<StopV2>>(AppConfiguration.StopsV2Path);
        _routes = FileHelper.ImportFromJsonFile<List<Route>>(AppConfiguration.RoutesPath);
        _routeV2S = FileHelper.ImportFromJsonFile<List<RouteV2>>(AppConfiguration.RoutesV2Path);
        _routeTimes = FileHelper.ImportFromJsonFile<List<RouteTimes>>(AppConfiguration.RouteTimesPath);
    }

    [TearDown]
    public void TearDown()
    {
        _synchronizer = null;
        _routeRepository = null;
        _routeRepositoryV2 = null;
        _stopsRepositoryV2 = null;
        _stopsRepository = null;
        _request = null;
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, null);
    }

    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestCreateFromEmptyDb(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _synchronizer.SynchronizeStaticData(_request);

        var createdStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, createdStops.Count);

        var createdStopsV2 = _stopsRepositoryV2.GetAll();
        Assert.AreEqual(_stopV2S.Count, createdStopsV2.Count);

        var createdRouteTimes = _routeRepository.GetAllRouteTimes();
        Assert.AreEqual(_routeTimes.Count, createdRouteTimes.Count);

        var createdRoutes = _routeRepository.GetAllRoutes();
        Assert.AreEqual(_routes.Count, createdRoutes.Count);

        var createdRouteV2S = _routeRepositoryV2.GetRoutes();
        Assert.AreEqual(_routeV2S.Count, createdRouteV2S.Count);
    }
}