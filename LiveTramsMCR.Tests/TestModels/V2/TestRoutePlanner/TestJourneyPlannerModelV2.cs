using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;
using LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using LiveTramsMCR.Tests.TestModels.V1.TestServices;
using NUnit.Framework;

using static LiveTramsMCR.Tests.Configuration.TestAppConfiguration;

namespace LiveTramsMCR.Tests.TestModels.V2.TestRoutePlanner;

public class TestJourneyPlannerModelV2 : BaseNunitTest
{
    private const string JourneyPlannerV2WithInterchangeResponsePath = "../../../Resources/TestJourneyPlannerV2/AltrinchamPiccadillyAshtonServices.json";
    private const string JourneyPlannerV2ResponsePath = "../../../Resources/TestJourneyPlannerV2/AltrinchamSaleServices.json";
    private const string StopsV1ResourcePath = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RoutesV2ResourcePath = "../../../Resources/RoutesV2.json";
    private const string StopsV2ResourcePath = "../../../Resources/StopsV2.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private List<StopV2> _importedStopV2S = new();
    private List<RouteV2> _importedRouteV2S = new();
    private IJourneyPlannerModelV2? _journeyPlannerModelV2Interchange;
    private IJourneyPlannerModelV2? _journeyPlannerModelV2;
    private IJourneyPlannerV2? _journeyPlannerV2;
    private INextServiceIdentifierV2? _nextServiceIdentifierV2;
    private ServiceProcessor? _serviceProcessor;
    private ServiceProcessor? _serviceProcessorWithInterchange;
    private StopLookupV2? _stopLookupV2;

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

        var stopsV1Repository = TestHelper.GetService<IStopsRepository>();
        MongoHelper.CreateRecords(AppConfiguration.StopsCollectionName, stopsV1);
        
        var stopsV2Loader = new StopV2Loader(resourcesConfig);
        _importedStopV2S = stopsV2Loader.ImportStops();

        var stopsRepositoryV2 = TestHelper.GetService<IStopsRepositoryV2>();
        MongoHelper.CreateRecords(AppConfiguration.StopsV2CollectionName, _importedStopV2S);
        
        _stopLookupV2 = new StopLookupV2(stopsRepositoryV2);

        var routesV1Loader = new RouteLoader(resourcesConfig, stopsV1);
        var routesV1 = routesV1Loader.ImportRoutes();

        var routesV2Loader = new RouteV2Loader(resourcesConfig);
        _importedRouteV2S = routesV2Loader.ImportRoutes();

        var routeTimesLoader = new RouteTimesLoader(resourcesConfig);
        var routeTimes = routeTimesLoader.ImportRouteTimes();

        var routeRepositoryV1 = TestHelper.GetService<IRouteRepository>();
        MongoHelper.CreateRecords(AppConfiguration.RoutesCollectionName, routesV1);
        MongoHelper.CreateRecords(AppConfiguration.RouteTimesCollectionName, routeTimes);
        
        var routeRepositoryV2 = TestHelper.GetService<IRouteRepositoryV2>();
        MongoHelper.CreateRecords(AppConfiguration.RoutesV2CollectionName, _importedRouteV2S);

        var journeyVisualiserV2 = new JourneyVisualiserV2();

        _journeyPlannerV2 = new JourneyPlannerV2(routeRepositoryV1, routeRepositoryV2);

        var mockInterchangeHttpResponse =
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(
                HttpStatusCode.OK, 
                JourneyPlannerV2WithInterchangeResponsePath);
        var mockHttpResponse = ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(
            HttpStatusCode.OK,
            JourneyPlannerV2ResponsePath);
        
        var serviceRequesterWithInterchange = new MockServiceRequester(mockInterchangeHttpResponse!);
        var serviceRequester = new MockServiceRequester(mockHttpResponse!);
        
        _serviceProcessorWithInterchange = new ServiceProcessor(serviceRequesterWithInterchange, stopsV1Repository);
        _serviceProcessor = new ServiceProcessor(serviceRequester, stopsV1Repository);
        _nextServiceIdentifierV2 = new NextServiceIdentifierV2();

        _journeyPlannerModelV2Interchange = new JourneyPlannerModelV2(
            _stopLookupV2, 
            _journeyPlannerV2, 
            journeyVisualiserV2, 
            _nextServiceIdentifierV2, 
            _serviceProcessorWithInterchange);

        _journeyPlannerModelV2 = new JourneyPlannerModelV2(
            _stopLookupV2, 
            _journeyPlannerV2, 
            journeyVisualiserV2, 
            _nextServiceIdentifierV2, 
            _serviceProcessor);
    }

    [Test]
    public void TestIdentifyRouteOnSameLine()
    {
        var response = _journeyPlannerModelV2?.PlanJourney("ALT", "SAL");
        Assert.IsNotNull(response);
        var plannedJourney = response?.PlannedJourney;
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
        var expectedStopsFromOriginNames = new List<string>
        {
            "Navigation Road", "Timperley", "Brooklands"
        };
        var actualStopNamesFromOrigin = plannedJourney?.StopsFromOrigin.Select(stop => stop.StopName).ToList();
        Assert.AreEqual(expectedStopsFromOriginNames, actualStopNamesFromOrigin);
        
        Assert.IsNotNull(response?.VisualisedJourney);
        var polylinesFromOrigin = response?.VisualisedJourney.PolylineFromOrigin;
        var purpleRoute = _importedRouteV2S.Single(route => route.Name == "Purple");
        var allPolyLineCoordinatesExistOnRoute = polylinesFromOrigin?.All(coord => 
            purpleRoute.PolylineCoordinates.Exists(pr =>
                Math.Abs(coord.Latitude - pr!.Latitude) < RouteCoordinateTolerance &&
                Math.Abs(coord.Longitude - pr.Longitude) < RouteCoordinateTolerance)
        );
        Assert.IsTrue(allPolyLineCoordinatesExistOnRoute);
        
        Assert.IsNotNull(response?.NextService);
        var nextService = response?.NextService;
        Assert.AreEqual(5, nextService?.Wait);
        var expectedNextServiceStopKeys = new StopKeysV2
        {
            StopName = "Piccadilly",
            Tlaref = "PIC"
        };
        Assert.AreEqual(expectedNextServiceStopKeys, nextService?.Destination);
        
        var messages = response?.ServiceUpdates;
        Assert.IsNotNull(messages);
        Assert.AreEqual(1, messages?.Count);
        
        var travelZones = response?.TravelZones;
        Assert.IsNotNull(travelZones);
        Assert.AreEqual(2, travelZones?.Count);
        var expectedTravelZones = new List<int>
        {
            3, 4
        };
        CollectionAssert.AreEqual(expectedTravelZones, travelZones);
    }

    [Test]
    public void TestIdentifyRouteWithInterchange()
    {
        var response = _journeyPlannerModelV2Interchange?.PlanJourney(
            "ALT",
            "ASH");
        Assert.IsNotNull(response);
        var plannedJourney = response?.PlannedJourney;
        var altrinchamStop = _importedStopV2S.Single(stop => stop.StopName == "Altrincham");
        var ashtonStop = _importedStopV2S.Single(stop => stop.StopName == "Ashton-Under-Lyne");
        var piccadillyStop = _importedStopV2S.Single(stop => stop.StopName == "Piccadilly");

        // Assert basic route properties
        Assert.IsNotNull(plannedJourney);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        Assert.AreEqual(altrinchamStop, plannedJourney?.OriginStop);
        Assert.AreEqual(ashtonStop, plannedJourney?.DestinationStop);

        // Assert routes from origin
        Assert.AreEqual(12, plannedJourney?.StopsFromOrigin.Count);
        var expectedStopNamesFromOrigin = new List<string>
        {
            "Navigation Road",
            "Timperley",
            "Brooklands",
            "Sale",
            "Dane Road",
            "Stretford",
            "Old Trafford",
            "Trafford Bar",
            "Cornbrook",
            "Deansgate - Castlefield",
            "St Peter's Square",
            "Piccadilly Gardens"
        };
        var actualStopNamesFromOrigin = plannedJourney?.StopsFromOrigin.Select(stop => stop.StopName).ToList();
        Assert.AreEqual(expectedStopNamesFromOrigin, actualStopNamesFromOrigin);
        Assert.AreEqual(1, plannedJourney?.RoutesFromOrigin.Count);
        Assert.AreEqual("Purple", plannedJourney?.RoutesFromOrigin.FirstOrDefault()?.Name);

        // Assert Interchange
        Assert.IsNotNull(plannedJourney?.InterchangeStop);
        Assert.AreEqual(piccadillyStop, plannedJourney?.InterchangeStop);

        // Assert routes from interchange
        Assert.AreEqual(11, plannedJourney?.StopsFromInterchange.Count);
        var expectedStopNamesFromInterchange = new List<string>
        {
            "New Islington",
            "Holt Town",
            "Etihad Campus",
            "Velopark",
            "Clayton Hall",
            "Edge Lane",
            "Cemetery Road",
            "Droylsden",
            "Audenshaw",
            "Ashton Moss",
            "Ashton West"
        };
        var actualStopNamesFromInterchange = plannedJourney?.StopsFromInterchange.Select(stop => stop.StopName).ToList();
        Assert.AreEqual(expectedStopNamesFromInterchange, actualStopNamesFromInterchange);
        Assert.AreEqual(1, plannedJourney?.RoutesFromInterchange.Count);
        Assert.AreEqual("Blue", plannedJourney?.RoutesFromInterchange.Single().Name);
        
        Assert.IsNotNull(response?.VisualisedJourney);
        var polylinesFromOrigin = response?.VisualisedJourney.PolylineFromOrigin;
        var purpleRoute = _importedRouteV2S.Single(route => route.Name == "Purple");
        var allPolyLineCoordinatesExistOnPurpleRoute = polylinesFromOrigin?.All(coord => 
            purpleRoute.PolylineCoordinates.Exists(pr =>
                Math.Abs(coord.Latitude - pr!.Latitude) < RouteCoordinateTolerance &&
                Math.Abs(coord.Longitude - pr.Longitude) < RouteCoordinateTolerance)
        );
        Assert.IsTrue(allPolyLineCoordinatesExistOnPurpleRoute);

        var polylinesFromInterchange = response?.VisualisedJourney.PolylineFromInterchange;
        Assert.IsNotNull(polylinesFromInterchange);
        var blueRoute = _importedRouteV2S.Single(route => route.Name == "Blue");
        var allPolyLineCoordinatesFromInterchangeExistOnBlueRoute = polylinesFromInterchange?.All(coord => 
            blueRoute.PolylineCoordinates.Exists(pr =>
                Math.Abs(coord.Latitude - pr!.Latitude) < RouteCoordinateTolerance &&
                Math.Abs(coord.Longitude - pr.Longitude) < RouteCoordinateTolerance)
        );
        Assert.IsTrue(allPolyLineCoordinatesFromInterchangeExistOnBlueRoute);

        var nextService = response?.NextService;
        Assert.IsNotNull(nextService);
        Assert.AreEqual(1, nextService?.Wait);
        var expectedInterchangeStopKeys = new StopKeysV2
        {
            StopName = "Piccadilly",
            Tlaref = "PIC"
        };
        Assert.AreEqual(expectedInterchangeStopKeys, nextService?.Destination);

        var messages = response?.ServiceUpdates;
        Assert.IsNotNull(messages);
        Assert.AreEqual(1, messages?.Count);

        var travelZones = response?.TravelZones;
        Assert.IsNotNull(travelZones);
        Assert.AreEqual(4, travelZones?.Count);
        var expectedTravelZones = new List<int>
        {
            1, 2, 3, 4
        };
        CollectionAssert.AreEqual(expectedTravelZones, travelZones);
    }

    [Test]
    public void TestNullOrigin()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _journeyPlannerModelV2Interchange?.PlanJourney(null, "Altrincham");
            });
    }

    [Test]
    public void TestNullDestination()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _journeyPlannerModelV2Interchange?.PlanJourney("Altrincham", null);
            });
    }

    [Test]
    public void TestInvalidOrigin()
    {
        Assert.Throws(Is.TypeOf<ArgumentException>(),
            delegate
            {
                var unused = _journeyPlannerModelV2Interchange?.PlanJourney("---", "Altrincham");
            });
    }

    [Test]
    public void TestInvalidDestination()
    {
        Assert.Throws(Is.TypeOf<ArgumentException>(),
            delegate
            {
                var unused = _journeyPlannerModelV2Interchange?.PlanJourney("Altrincham", "---");
            });
    }
}