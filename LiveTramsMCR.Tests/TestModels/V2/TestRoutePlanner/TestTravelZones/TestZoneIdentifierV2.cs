using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.RoutePlanner.TravelZones;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V2.TestRoutePlanner.TestTravelZones;

/// <summary>
/// Test class for the travel zone identifier.
/// </summary>
public class TestZoneIdentifierV2
{
    private const string StopsV1ResourcePath = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RoutesV2ResourcePath = "../../../Resources/RoutesV2.json";
    private const string StopsV2ResourcePath = "../../../Resources/StopsV2.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private List<Stop>? _importedStopsV1S;
    private List<StopV2>? _importedStopsV2S;
    private JourneyPlannerV2? _journeyPlanner;
    private MockRouteRepository? _mockRouteRepository;
    private MockRouteRepositoryV2? _mockRouteRepositoryV2;
    private MockStopsRepositoryV2? _mockStopsRepositoryV2;
    private RouteLoader? _routeLoader;
    private List<Route>? _routes;
    private List<RouteV2>? _routesV2S;
    private List<RouteTimes>? _routeTimes;
    private RouteTimesLoader? _routeTimesLoader;
    private RouteV2Loader? _routeV2Loader;
    private StopLoader? _stopLoader;
    private StopV2Loader? _stopV2Loader;
    private ResourcesConfig? _validResourcesConfig;
    private ZoneIdentifierV2? _zoneIdentifierV2;

    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopsV1ResourcePath,
            RoutesV2ResourcePath = RoutesV2ResourcePath,
            RouteTimesPath = RouteTimesPath,
            StopV2ResourcePath = StopsV2ResourcePath
        };

        _stopLoader = new StopLoader(_validResourcesConfig);
        _importedStopsV1S = _stopLoader.ImportStops();

        _stopV2Loader = new StopV2Loader(_validResourcesConfig);
        _importedStopsV2S = _stopV2Loader.ImportStops();

        _routeV2Loader = new RouteV2Loader(_validResourcesConfig);
        _routesV2S = _routeV2Loader.ImportRoutes();

        _routeLoader = new RouteLoader(_validResourcesConfig, _importedStopsV1S);
        _routes = _routeLoader.ImportRoutes();

        _routeTimesLoader = new RouteTimesLoader(_validResourcesConfig);
        _routeTimes = _routeTimesLoader.ImportRouteTimes();
        _mockStopsRepositoryV2 = new MockStopsRepositoryV2(_importedStopsV2S);
        _mockRouteRepository = new MockRouteRepository(_routes, _routeTimes);
        _mockRouteRepositoryV2 = new MockRouteRepositoryV2(_routesV2S, _mockStopsRepositoryV2);
        _journeyPlanner = new JourneyPlannerV2(_mockRouteRepository, _mockRouteRepositoryV2);
        _zoneIdentifierV2 = new ZoneIdentifierV2();
    }

    [Test]
    public void TestIdentifyStopZonesSameZone()
    {
        var altrinchamStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "ALT");
        var navigationRoadStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "NAV");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, navigationRoadStop);
        var result = _zoneIdentifierV2?.IdentifyZonesForJourney(plannedJourney);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result?.Count);
        CollectionAssert.Contains(result,4);
    }

    [Test]
    public void TestIdentifyMultipleZoneJourneysNoInterchange()
    {
        var altrinchamStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "ALT");
        var saleStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "SAL");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, saleStop);
        var result = _zoneIdentifierV2?.IdentifyZonesForJourney(plannedJourney);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result?.Count);
        var expectedZones = new List<int>
        {
            3, 4
        };
        CollectionAssert.AreEqual(expectedZones, result);
    }

    [Test]
    public void TestIdentifyMultipleZoneWithInterchange()
    {
        var buryStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "BRY");
        var freeholdStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "FRE");
        var plannedJourney = _journeyPlanner?.PlanJourney(buryStop, freeholdStop);
        var result = _zoneIdentifierV2?.IdentifyZonesForJourney(plannedJourney);
        Assert.IsNotNull(result);
        Assert.AreEqual(4, result?.Count);
        var expectedZones = new List<int>
        {
            1, 2, 3, 4
        };
        CollectionAssert.AreEqual(expectedZones, result);
    }

    /// <summary>
    /// Test to identify zones when ending in a zone in the format
    /// a/b.
    /// </summary>
    [Test]
    public void TestIdentifyEndInCombinedZone()
    {
        var altrinchamStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "ALT");
        var stretfordStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "STR");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, stretfordStop);
        var result = _zoneIdentifierV2?.IdentifyZonesForJourney(plannedJourney);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result?.Count);
        var expectedZones = new List<int>
        {
            3, 4
        };
        CollectionAssert.AreEqual(expectedZones, result);
    }
    
    /// <summary>
    /// Test to identify zones when starting in a zone in the format
    /// a/b.
    /// </summary>
    [Test]
    public void TestIdentifyStartInCombinedZone()
    {
        var piccadillyStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "PIC");
        var stretfordStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "STR");
        var plannedJourney = _journeyPlanner?.PlanJourney(stretfordStop, piccadillyStop);
        var result = _zoneIdentifierV2?.IdentifyZonesForJourney(plannedJourney);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result?.Count);
        var expectedZones = new List<int>
        {
            1, 2
        };
        CollectionAssert.AreEqual(expectedZones, result);
    }

    /// <summary>
    /// Test to start on the left of a combined zone, e.g.
    /// starting in zone 3 and ending in zone 3/4
    /// </summary>
    [Test]
    public void TestStartOnLeftOfCombinedZone()
    {
        var saleStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "SAL");
        var brooklandsStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "BRK");
        var plannedJourney = _journeyPlanner?.PlanJourney(saleStop, brooklandsStop);
        var result = _zoneIdentifierV2?.IdentifyZonesForJourney(plannedJourney);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result?.Count);
        var expectedZones = new List<int>
        {
            3
        };
        CollectionAssert.AreEqual(expectedZones, result);
    }
    
    /// <summary>
    /// Test to start in the right of a combined zone, e.g.
    /// starting in zone 4 and ending in zone 3/4
    /// </summary>
    [Test]
    public void TestStartOnRightOfCombinedZone()
    {
        var altrinchamStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "ALT");
        var brooklandsStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "BRK");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, brooklandsStop);
        var result = _zoneIdentifierV2?.IdentifyZonesForJourney(plannedJourney);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result?.Count);
        var expectedZones = new List<int>
        {
            4
        };
        CollectionAssert.AreEqual(expectedZones, result);
    }

    [Test]
    public void TestJourneyAcrossAllZones()
    {
        var altrinchamStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "ALT");
        var brooklandsStop = _importedStopsV2S?.Single(stop => stop.Tlaref == "RTC");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, brooklandsStop);
        var result = _zoneIdentifierV2?.IdentifyZonesForJourney(plannedJourney);
        Assert.IsNotNull(result);
        Assert.AreEqual(4, result?.Count);
        var expectedZones = new List<int>
        {
            1, 2, 3, 4
        };
        CollectionAssert.AreEqual(expectedZones, result);
    }
}