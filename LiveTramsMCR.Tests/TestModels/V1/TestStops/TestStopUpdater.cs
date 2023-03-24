using System.Collections.Generic;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using LiveTramsMCR.Tests.TestModels.V1.TestServices;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestStops;

public class TestStopUpdater
{
    private const string StopResourcePathConst = "../../../Resources/TestStopUpdater/stops.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestStopUpdater/routes.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private const string ApiResponsePath = "../../../Resources/TestStopUpdater/ApiResponse.json";
    private ResourcesConfig? _resourcesConfig;
    private StopUpdater? _stopUpdater;
    private StopLoader? _stopLoader;
    private List<Stop>? _importedStops;
    private List<Route>? _importedRoutes;
    private List<RouteTimes>? _importedRouteTimes;
    private RouteLoader? _routeLoader;
    private RouteTimesLoader? _routeTimesLoader;
    private MultipleUnformattedServices? _multipleUnformattedServices;
    private IStopsRepository? _stopsRepository;
    private IRouteRepository? _routeRepository;
    
    [SetUp]
    public void SetUp()
    {
        _resourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesResourcePath,
            RouteTimesPath = RouteTimesPath
        };
        _stopLoader = new StopLoader(_resourcesConfig);
        _importedStops = _stopLoader.ImportStops();
        
        _routeLoader = new RouteLoader(_resourcesConfig, _importedStops);
        _importedRoutes = _routeLoader.ImportRoutes();

        _routeTimesLoader = new RouteTimesLoader(_resourcesConfig);
        _importedRouteTimes = _routeTimesLoader.ImportRouteTimes();
        
        _stopsRepository = new MockStopsRepository(_importedStops);
        _routeRepository = new MockRouteRepository(_importedRoutes, _importedRouteTimes);

        _multipleUnformattedServices = ImportServicesResponse.ImportMultipleUnformattedServices(ApiResponsePath);
        
        _stopUpdater = new StopUpdater(_stopsRepository, _routeRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _stopUpdater = null;
        _routeRepository = null;
        _stopsRepository = null;
        _importedRouteTimes = null;
        _routeTimesLoader = null;
        _importedRoutes = null;
        _routeLoader = null;
        _importedStops = null;
        _stopLoader = null;
        _resourcesConfig = null;
    }

    /// <summary>
    /// Test to update the stops in the stops repository.
    /// After running the method, the IDs in the stops should be updated
    /// </summary>
    [Test]
    public void TestUpdateStops()
    {
        // Expected IDs:
        // Example 1: 15588, 15589, 15590
        // Example 2: 15591, 15592
        // Example 3: 15593, 15594
        _stopUpdater?.UpdateStopIdsFromServices(_multipleUnformattedServices?.Value);
        
        var updatedStops = _stopsRepository?.GetAll();
        Assert.NotNull(updatedStops);
        Assert.AreEqual(3, updatedStops?.Count);
        var expectedExampleOneIds = new List<int>() {15588, 15589, 15590};
        Assert.AreEqual(expectedExampleOneIds, updatedStops?.Find(s => s.StopName == "Example 1")!.Ids);
        
        var expectedExampleTwoIds = new List<int>() {15591, 15592};
        Assert.AreEqual(expectedExampleTwoIds, updatedStops?.Find(s => s.StopName == "Example 2")!.Ids);
        
        var expectedExampleThreeIds = new List<int>() {15593, 15594};
        Assert.AreEqual(expectedExampleThreeIds, updatedStops?.Find(s => s.StopName == "Example 3")!.Ids);
    }

    /// <summary>
    /// Test to update the stops in the imported routes.
    /// This should update the IDs for all of the stops in each route.
    /// </summary>
    [Test]
    public void TestUpdateStopsInRoutes()
    {
        // Expected IDs:
        // Example 1: 15588, 15589, 15590
        // Example 2: 15591, 15592
        // Example 3: 15593, 15594
        _stopUpdater?.UpdateStopIdsFromServices(_multipleUnformattedServices?.Value);

        var updatedRoutes = _routeRepository?.GetAllRoutesAsync();
        Assert.NotNull(updatedRoutes);
        Assert.AreEqual(2, updatedRoutes?.Count);
        
        var expectedExampleOneIds = new List<int>() {15588, 15589, 15590};
        var expectedExampleTwoIds = new List<int>() {15591, 15592};
        var expectedExampleThreeIds = new List<int>() {15593, 15594};
        
        var greenRoute = updatedRoutes?.Find(r => r.Name == "Green");
        Assert.NotNull(greenRoute);
        Assert.AreEqual(2, greenRoute?.Stops.Count);
        var expectedExampleOne = greenRoute?.Stops.Find(s => s.StopName == "Example 1");
        var expectedExampleThree = greenRoute?.Stops.Find(s => s.StopName == "Example 3");
        Assert.AreEqual(expectedExampleOneIds, expectedExampleOne?.Ids);
        Assert.AreEqual(expectedExampleThreeIds, expectedExampleThree?.Ids);

        var purpleRoute = updatedRoutes?.Find(r => r.Name == "Purple");
        Assert.AreEqual(2, purpleRoute?.Stops.Count);
        expectedExampleOne = purpleRoute?.Stops.Find(s => s.StopName == "Example 1");
        var expectedExampleTwo = purpleRoute?.Stops.Find(s => s.StopName == "Example 2");
        Assert.AreEqual(expectedExampleOneIds, expectedExampleOne?.Ids);
        Assert.AreEqual(expectedExampleTwoIds, expectedExampleTwo?.Ids);

    }
}