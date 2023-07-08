using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V2.RoutePlanner;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V2.TestRoutePlanner;

public class TestJourneyPlannerModel
{
    private const string StopsV1ResourcePath = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RoutesV2ResourcePath = "../../../Resources/RoutesV2.json";
    private const string StopsV2ResourcePath = "../../../Resources/StopsV2.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private List<StopV2> _importedStopV2S = new List<StopV2>();
    private IJourneyPlannerModelV2? _journeyPlannerModelV2;
    private IJourneyPlannerV2? _journeyPlannerV2;
    private StopLookupV2? _stopLookupV2;
    
    [SetUp]
    public void SetUp()
    {
        var resourcesConfig = new ResourcesConfig()
        {
            StopResourcePath = StopsV1ResourcePath,
            RoutesV2ResourcePath = RoutesV2ResourcePath,
            RouteTimesPath = RouteTimesPath,
            StopV2ResourcePath = StopsV2ResourcePath
        };

        var stopsV1Loader = new StopLoader(resourcesConfig);
        var stopsV1 = stopsV1Loader.ImportStops();
        
        var stopsV2Loader = new StopV2Loader(resourcesConfig);
        _importedStopV2S = stopsV2Loader.ImportStops();
        
        var mockStopsV2Repository = new MockStopsRepositoryV2(_importedStopV2S);
        _stopLookupV2 = new StopLookupV2(mockStopsV2Repository);

        var routesV1Loader = new RouteLoader(resourcesConfig, stopsV1);
        var routesV1 = routesV1Loader.ImportRoutes();

        var routesV2Loader = new RouteV2Loader(resourcesConfig);
        var routesV2 = routesV2Loader.ImportRoutes();
        
        var routeTimesLoader = new RouteTimesLoader(resourcesConfig);
        var routeTimes = routeTimesLoader.ImportRouteTimes();

        var mockRouteRepositoryV1 = new MockRouteRepository(routesV1, routeTimes);
        var mockRouteRepositoryV2 = new MockRouteRepositoryV2(routesV2, mockStopsV2Repository);
        
        _journeyPlannerV2 = new JourneyPlannerV2(mockRouteRepositoryV1, mockRouteRepositoryV2);

        _journeyPlannerModelV2 = new JourneyPlannerModelV2(_stopLookupV2, _journeyPlannerV2);
    }

    [Test]
    public void TestIdentifyRouteOnSameLine()
    {
        var plannedJourney = _journeyPlannerModelV2?.PlanJourney("Altrincham", "Sale");
        var altrinchamStop = _importedStopV2S.Single(stop => stop.Tlaref == "ALT");
        var saleStop = _importedStopV2S.Single(stop => stop.StopName == "Sale");
        
        Assert.IsNotNull(plannedJourney);
        Assert.IsFalse(plannedJourney?.RequiresInterchange);
        Assert.AreEqual(altrinchamStop, plannedJourney?.OriginStop);
        Assert.AreEqual(saleStop, plannedJourney?.DestinationStop);
        Assert.AreEqual(2, plannedJourney?.RoutesFromOrigin.Count);
        var routeNamesFromOrigin = plannedJourney?.RoutesFromOrigin.Select(r => r.Name).ToList();
        Assert.Contains("Purple", routeNamesFromOrigin);
        Assert.Contains("Green", routeNamesFromOrigin);
        Assert.AreEqual(3, plannedJourney?.StopsFromOrigin.Count);
        var expectedStopsFromOriginNames = new List<string>()
        {
            "Navigation Road", "Timperley", "Brooklands"
        };
        var actualStopNamesFromOrigin = plannedJourney?.StopsFromOrigin.Select(stop => stop.StopName).ToList();
        Assert.AreEqual(expectedStopsFromOriginNames, actualStopNamesFromOrigin);
    }
}