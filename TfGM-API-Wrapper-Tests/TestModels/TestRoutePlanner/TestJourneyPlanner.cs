using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper_Tests.TestModels.TestRoutePlanner;

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
    private ResourcesConfig? _validResourcesConfig;
    private StopLoader? _stopLoader;
    private List<Stop>? _importedStops;
    private RouteLoader? _routeLoader;
    private List<Route>? _routes;
    private JourneyPlanner? _journeyPlanner;
    
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
            RoutesResourcePath = RoutesResourcePath
        };
        
        _stopLoader = new StopLoader(_validResourcesConfig);
        _importedStops = _stopLoader.ImportStops();

        _routeLoader = new RouteLoader(_validResourcesConfig, _importedStops);
        _routes = _routeLoader.ImportRoutes();
        _journeyPlanner = new JourneyPlanner(_routes);
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
}