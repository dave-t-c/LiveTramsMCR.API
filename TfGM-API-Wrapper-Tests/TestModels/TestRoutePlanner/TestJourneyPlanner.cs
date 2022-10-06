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
}