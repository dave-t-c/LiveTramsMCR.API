using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V2.TestRoutePlanner.TestVisualisation;

public class TestJourneyVisualiserV2 : BaseNunitTest
{
    private const string StopsV1ResourcePath = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RoutesV2ResourcePath = "../../../Resources/RoutesV2.json";
    private const string StopsV2ResourcePath = "../../../Resources/StopsV2.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private List<StopV2> _importedStopV2S = new();
    private IJourneyPlannerV2? _journeyPlannerV2;
    private IJourneyVisualiserV2? _journeyVisualiserV2;
    private StopV2? _altrinchamStopV2;
    private StopV2? _ashtonStopV2;
    private StopV2? _saleStopV2;
    private RouteV2? _purpleRouteV2;
    private RouteV2? _blueRouteV2;
    
    [SetUp]
    public void SetUp()
    {
        var resourcesConfig = new ResourcesConfig
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

        var stopsRepositoryV2 = TestHelper.GetService<IStopsRepositoryV2>();
        MongoHelper.CreateRecords(AppConfiguration.StopsV2CollectionName, _importedStopV2S);

        var routesV1Loader = new RouteLoader(resourcesConfig, stopsV1);
        var routesV1 = routesV1Loader.ImportRoutes();

        var routesV2Loader = new RouteV2Loader(resourcesConfig);
        var routesV2 = routesV2Loader.ImportRoutes();

        var routeTimesLoader = new RouteTimesLoader(resourcesConfig);
        var routeTimes = routeTimesLoader.ImportRouteTimes();

        var routeRepositoryV1 = TestHelper.GetService<IRouteRepository>();
        MongoHelper.CreateRecords(AppConfiguration.RoutesCollectionName, routesV1);
        MongoHelper.CreateRecords(AppConfiguration.RouteTimesCollectionName, routeTimes);
        
        var routeRepositoryV2 = TestHelper.GetService<IRouteRepositoryV2>();
        MongoHelper.CreateRecords(AppConfiguration.RoutesV2CollectionName, routesV2);

        _journeyPlannerV2 = new JourneyPlannerV2(routeRepositoryV1, routeRepositoryV2);
        _journeyVisualiserV2 = new JourneyVisualiserV2();
        
        _altrinchamStopV2 = _importedStopV2S.Single(stop => stop.Tlaref == "ALT");
        _ashtonStopV2 = _importedStopV2S.Single(stop => stop.StopName == "Ashton-Under-Lyne");
        _saleStopV2 = _importedStopV2S.Single(stop => stop.StopName == "Sale");
        _purpleRouteV2 = routesV2.Single(route => route.Name == "Purple");
        _blueRouteV2 = routesV2.Single(route => route.Name == "Blue");
    }

    [Test]
    public void TestGenerateVisualisationForJourneyWithoutInterchange()
    {
        var plannedJourney = _journeyPlannerV2?.PlanJourney(_altrinchamStopV2, _saleStopV2);
        Assert.NotNull(plannedJourney);

        var visualisedJourney = _journeyVisualiserV2?.VisualiseJourney(plannedJourney);
        Assert.NotNull(visualisedJourney);
        Assert.NotNull(visualisedJourney?.PolylineFromOrigin);
        Assert.IsNull(visualisedJourney?.PolylineFromInterchange);
        
        var expectedInitialPosition = _purpleRouteV2?.PolylineCoordinates.First();
        var expectedFinalPosition = new RouteV2.RouteCoordinate()
        {
            Latitude = 53.423642572600002,  
            Longitude = -2.3197075925999999
        };

        var actualInitialPosition = visualisedJourney?.PolylineFromOrigin.First();
        Assert.AreEqual(expectedInitialPosition!.Latitude, actualInitialPosition!.Latitude);
        Assert.AreEqual(expectedInitialPosition.Longitude, actualInitialPosition.Longitude);

        var actualFinalPosition = visualisedJourney?.PolylineFromOrigin.Last();
        Assert.AreEqual(expectedFinalPosition.Latitude, actualFinalPosition!.Latitude);
        Assert.AreEqual(expectedFinalPosition.Longitude, actualFinalPosition.Longitude);
    }

    [Test]
    public void TestGenerateVisualisationFromJourneyWithInterchange()
    {
        var plannedJourney = _journeyPlannerV2?.PlanJourney(_altrinchamStopV2, _ashtonStopV2);
        Assert.NotNull(plannedJourney);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        
        var visualisedJourney = _journeyVisualiserV2?.VisualiseJourney(plannedJourney);
        Assert.NotNull(visualisedJourney);
        Assert.NotNull(visualisedJourney?.PolylineFromOrigin);
        Assert.NotNull(visualisedJourney?.PolylineFromInterchange);
        
        var expectedInitialPosition = _purpleRouteV2?.PolylineCoordinates.First();

        var actualInitialPosition = visualisedJourney?.PolylineFromOrigin.First();
        Assert.AreEqual(expectedInitialPosition!.Latitude, actualInitialPosition!.Latitude);
        Assert.AreEqual(expectedInitialPosition.Longitude, actualInitialPosition.Longitude);
        
        // Blue line is ordered with Ashton first so in the returned polyline from interchange, the line 
        // will be Ashton to Piccadilly
        var expectedInitialPositionInterchange = _blueRouteV2?.PolylineCoordinates.First();
        var actualInitialPositionInterchange = visualisedJourney?.PolylineFromInterchange.First();
        Assert.AreEqual(expectedInitialPositionInterchange!.Latitude, actualInitialPositionInterchange!.Latitude);
        Assert.AreEqual(expectedInitialPositionInterchange.Longitude, actualInitialPositionInterchange.Longitude);
    }
}