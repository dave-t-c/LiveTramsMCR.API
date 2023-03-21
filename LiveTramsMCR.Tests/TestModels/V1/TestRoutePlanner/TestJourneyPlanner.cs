using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;
using ResourcesConfig = LiveTramsMCR.Tests.Resources.ResourceLoaders.ResourcesConfig;
using StopLoader = LiveTramsMCR.Tests.Resources.ResourceLoaders.StopLoader;

namespace LiveTramsMCR.Tests.TestModels.V1.TestRoutePlanner;

/// <summary>
/// Test class for the route planner, a class for
/// identifying routes between given stops
/// </summary>
public class TestRoutePlanner
{
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private ResourcesConfig? _validResourcesConfig;
    private StopLoader? _stopLoader;
    private List<Stop>? _importedStops;
    private RouteLoader? _routeLoader;
    private List<Route>? _routes;
    private JourneyPlanner? _journeyPlanner;
    private List<RouteTimes>? _routeTimes;
    private MockRouteRepository? _mockRouteRepository;
    private RouteTimesLoader? _routeTimesLoader;
    
    /// <summary>
    /// Sets up the required resources for testing route planning,
    /// such as importing stops and routes.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        //To make searching between stops easier, 
        //Stops are imported and then selected.
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesResourcePath,
            RouteTimesPath = RouteTimesPath
        };
        
        _stopLoader = new StopLoader(_validResourcesConfig);
        _importedStops = _stopLoader.ImportStops();

        _routeLoader = new RouteLoader(_validResourcesConfig, _importedStops);
        _routes = _routeLoader.ImportRoutes();

        _routeTimesLoader = new RouteTimesLoader(_validResourcesConfig);
        _routeTimes = _routeTimesLoader.ImportRouteTimes();
        _mockRouteRepository = new MockRouteRepository(_routes, _routeTimes);
        _journeyPlanner = new JourneyPlanner(_mockRouteRepository);
    }

    /// <summary>
    /// Clear configs after each test to avoid cross-test bugs.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
    }

    /// <summary>
    /// Test to plan a route on a single line between Altrincham and Sale
    /// This should return a planned route POCO with the expected origin and destination stops.
    /// This should return that the journey does not require an interchange
    /// and has two possible routes from the origin.
    /// </summary>
    [Test]
    public void TestIdentifyRouteOnSameLine()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var saleStop = _importedStops?.First(stop => stop.StopName == "Sale");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, saleStop);
        //Should be two possible routes from the origin, purple and green
       Assert.IsNotNull(plannedJourney);
       Assert.IsFalse(plannedJourney?.RequiresInterchange);
       Assert.IsNotNull(plannedJourney?.RoutesFromOrigin);
       Assert.AreEqual(2, plannedJourney?.RoutesFromOrigin.Count);
       Assert.IsTrue(plannedJourney?.RoutesFromOrigin.Any(route => route.Name == "Green"));
       Assert.IsTrue(plannedJourney?.RoutesFromOrigin.Any(route => route.Name == "Purple"));
    }


    /// <summary>
    /// Test to identify a route where an interchange is required.
    /// This should be true.
    /// </summary>
    [Test]
    public void TestPlanRouteInterchangeRequired()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport");
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        var plannedJourney = _journeyPlanner?.PlanJourney(airportStop, buryStop);
        Assert.NotNull(plannedJourney);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        Assert.IsNotNull(plannedJourney?.RoutesFromInterchange);
        Assert.AreEqual(2, plannedJourney?.RoutesFromInterchange.Count);
        Assert.IsTrue(plannedJourney?.RoutesFromInterchange.Any(route => route.Name == "Green"));
        Assert.IsTrue(plannedJourney?.RoutesFromInterchange.Any(route => route.Name == "Yellow"));
    }
    
    /// <summary>
    /// Test to identify a route between the airport and bury.
    /// This should include the expected stops before and after the interchange.
    /// </summary>
    [Test]
    public void TestIdentifyJourneyAirportBuryStops()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport");
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        var plannedJourney = _journeyPlanner?.PlanJourney(airportStop, buryStop);
        Assert.IsNotNull(plannedJourney);
        Assert.IsNotNull(plannedJourney?.StopsFromOrigin);
        Assert.IsNotNull(plannedJourney?.StopsFromInterchange);
        Assert.IsNotNull(plannedJourney?.InterchangeStop);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        var victoriaStop = _importedStops?.First(stop => stop.StopName == "Victoria");
        Assert.AreEqual(victoriaStop, plannedJourney?.InterchangeStop);
        var originStops = plannedJourney?.StopsFromOrigin;
        var interchangeStops = plannedJourney?.StopsFromInterchange;
        Assert.AreEqual(23, originStops?.Count);
        Assert.AreEqual(9, interchangeStops?.Count);
        var destinationsFromOrigin = plannedJourney?.TerminiFromOrigin;
        var destinationsFromInterchange = plannedJourney?.TerminiFromInterchange;
        Assert.AreEqual(1, destinationsFromOrigin?.Count);
        Assert.AreEqual(1, destinationsFromInterchange?.Count);
        
    }

    /// <summary>
    /// Test to identify the expected route between Altrincham and
    /// Ashton.
    /// This should identify Piccadilly as the interchange stop.
    /// </summary>
    [Test]
    public void TestIdentifyAltrinchamAshton()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var ashtonStop = _importedStops?.First(stop => stop.StopName == "Ashton-Under-Lyne");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, ashtonStop);
        Assert.IsNotNull(plannedJourney);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        var piccadillyStop = _importedStops?.First(stop => stop.StopName == "Piccadilly");
        Assert.IsNotNull(plannedJourney?.InterchangeStop);
        Assert.AreEqual(piccadillyStop, plannedJourney?.InterchangeStop);
        Assert.AreEqual(1, plannedJourney?.RoutesFromOrigin.Count);
        Assert.AreEqual(1, plannedJourney?.RoutesFromInterchange.Count);
        Assert.AreEqual(12, plannedJourney?.StopsFromOrigin.Count);
        Assert.AreEqual(11, plannedJourney?.StopsFromInterchange.Count);
    }


    /// <summary>
    /// Test to identify a route between Ashton and Altrincham.
    /// This should identify Cornbrook as the interchange stop.
    /// There should also be two routes from the interchange stop.
    /// </summary>
    [Test]
    public void TestIdentifyAshtonAltrincham()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var ashtonStop = _importedStops?.First(stop => stop.StopName == "Ashton-Under-Lyne");
        var plannedJourney = _journeyPlanner?.PlanJourney(ashtonStop, altrinchamStop);
        Assert.IsNotNull(plannedJourney);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        var cornbrookStop = _importedStops?.First(stop => stop.StopName == "Cornbrook");
        Assert.AreEqual(cornbrookStop, plannedJourney?.InterchangeStop);
        Assert.AreEqual(1, plannedJourney?.RoutesFromOrigin.Count);
        Assert.AreEqual(2, plannedJourney?.RoutesFromInterchange.Count);
        Assert.AreEqual(15, plannedJourney?.StopsFromOrigin.Count);
        Assert.AreEqual("Ashton West", plannedJourney?.StopsFromOrigin.First().StopName);
        Assert.AreEqual("Deansgate - Castlefield", plannedJourney?.StopsFromOrigin.Last().StopName);
        Assert.AreEqual(8, plannedJourney?.StopsFromInterchange.Count);
        Assert.AreEqual("Trafford Bar", plannedJourney?.StopsFromInterchange.First().StopName);
        Assert.AreEqual("Navigation Road", plannedJourney?.StopsFromInterchange.Last().StopName);
    }

    /// <summary>
    /// Test to identify a route between Ashton and Bury.
    /// This should identify Piccadilly Gardens as the interchange, and the Yellow line.
    /// </summary>
    [Test]
    public void TestIdentifyAshtonBury()
    {
        var ashtonStop = _importedStops?.First(stop => stop.StopName == "Ashton-Under-Lyne");
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        var plannedJourney = _journeyPlanner?.PlanJourney(ashtonStop, buryStop);
        Assert.IsNotNull(plannedJourney);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        var piccadillyGardensStop = _importedStops?.First(stop => stop.StopName == "Piccadilly Gardens");
        Assert.AreEqual(piccadillyGardensStop, plannedJourney?.InterchangeStop);
        Assert.AreEqual(1, plannedJourney?.RoutesFromOrigin.Count);
        Assert.AreEqual(12, plannedJourney?.StopsFromOrigin.Count);
        Assert.AreEqual("Ashton West", plannedJourney?.StopsFromOrigin.First().StopName);
        Assert.AreEqual("Piccadilly", plannedJourney?.StopsFromOrigin.Last().StopName);
        Assert.AreEqual(12, plannedJourney?.StopsFromInterchange.Count);
        Assert.AreEqual("Market Street", plannedJourney?.StopsFromInterchange.First().StopName);
        Assert.AreEqual("Radcliffe", plannedJourney?.StopsFromInterchange.Last().StopName);
    }

    /// <summary>
    /// Test to identify a route between Didsbury and Rochdale.
    /// This should identify an interchange is not required,
    /// and there is only one possible route.
    /// </summary>
    [Test]
    public void TestIdentifyDidsburyRochdale()
    {
        var didsburyStop =  _importedStops?.First(stop => stop.StopName == "East Didsbury");
        var rochdaleStop = _importedStops?.First(stop => stop.StopName == "Rochdale Town Centre");
        var plannedJourney = _journeyPlanner?.PlanJourney(didsburyStop, rochdaleStop);
        Assert.IsNotNull(plannedJourney);
        Assert.IsFalse(plannedJourney?.RequiresInterchange);
        Assert.AreEqual(1, plannedJourney?.RoutesFromOrigin.Count);
        Assert.AreEqual("Pink", plannedJourney?.RoutesFromOrigin.First().Name);
        Assert.AreEqual(31, plannedJourney?.StopsFromOrigin.Count);
        Assert.AreEqual("Didsbury Village", plannedJourney?.StopsFromOrigin.First().StopName);
        Assert.AreEqual("Rochdale Railway Station", plannedJourney?.StopsFromOrigin.Last().StopName);
    }


    /// <summary>
    /// Test to identify a journey between Didsbury and shaw and crompton.
    /// This should identify that no interchange is required, 
    /// </summary>
    [Test]
    public void TestIdentifyDidsburyShawAndCrompton()
    {
        var didsburyStop =  _importedStops?.First(stop => stop.StopName == "East Didsbury");
        var shawStop =  _importedStops?.First(stop => stop.StopName == "Shaw and Crompton");
        var plannedJourney = _journeyPlanner?.PlanJourney(didsburyStop, shawStop);
        Assert.IsNotNull(plannedJourney);
        Assert.IsFalse(plannedJourney?.RequiresInterchange);
        Assert.AreEqual(2, plannedJourney?.RoutesFromOrigin.Count);
        Assert.IsTrue(plannedJourney?.RoutesFromOrigin.Any(route => route.Name == "Pink"));
        Assert.IsTrue(plannedJourney?.RoutesFromOrigin.Any(route => route.Name == "Grey"));
        Assert.AreEqual(25, plannedJourney?.StopsFromOrigin.Count);
        Assert.AreEqual("Didsbury Village", plannedJourney?.StopsFromOrigin.First().StopName);
        Assert.AreEqual("Derker", plannedJourney?.StopsFromOrigin.Last().StopName);
    }

    /// <summary>
    /// Test to identify the route time between Altrincham and Piccadilly.
    /// This does not require an interchange  
    /// </summary>
    [Test]
    public void TestIdentifyAltrinchamPiccadillyTimes()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var piccadillyStop = _importedStops?.First(stop => stop.StopName == "Piccadilly");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, piccadillyStop);
        Assert.IsNotNull(plannedJourney);
        Assert.IsFalse(plannedJourney?.RequiresInterchange);
        Assert.AreEqual(32, plannedJourney?.MinutesFromOrigin);
    }

    /// <summary>
    /// Test to identify the route time between Altrincham and Asthon.
    /// This requires an interchange so both minutes from origin and minutes
    /// from interchange should not be 0.
    /// </summary>
    [Test]
    public void TestIdentifyAltrinchamAshtonTimes()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var ashtonStop = _importedStops?.First(stop => stop.StopName == "Ashton-Under-Lyne");
        var plannedJourney = _journeyPlanner?.PlanJourney(altrinchamStop, ashtonStop);
        Assert.IsNotNull(plannedJourney);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        var piccadillyStop = _importedStops?.First(stop => stop.StopName == "Piccadilly");
        Assert.AreEqual(piccadillyStop, plannedJourney?.InterchangeStop);
        Assert.AreEqual(32, plannedJourney?.MinutesFromOrigin);
        Assert.AreEqual(28, plannedJourney?.MinutesFromInterchange);
        Assert.AreEqual(60, plannedJourney?.TotalJourneyTimeMinutes);
    }
    
}