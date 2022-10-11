using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper_Tests.TestModels.TestRoutePlanner;

/// <summary>
/// Test class for the journey planner model.
/// </summary>
public class TestJourneyPlannerModel
{
    
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private ResourcesConfig? _validResourcesConfig;
    private ImportedResources? _importedResources;
    private ResourceLoader? _resourceLoader;
    private List<Stop>? _importedStops;
    private List<Route>? _routes;
    private JourneyPlanner? _journeyPlanner;
    private JourneyPlannerModel? _journeyPlannerModel;
    
    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesResourcePath
        };

        _resourceLoader = new ResourceLoader(_validResourcesConfig);
        _importedResources = _resourceLoader.ImportResources();

        _importedStops = _importedResources?.ImportedStops;

        _routes = _importedResources?.ImportedRoutes;
        _journeyPlanner = new JourneyPlanner(_routes);
        _journeyPlannerModel = new JourneyPlannerModel(_importedResources, _journeyPlanner);
    }

    [TearDown]
    public void TearDown()
    {
        _routes = null;
        _importedStops = null;
        _resourceLoader = null;
        _validResourcesConfig = null;
    }

    /// <summary>
    /// Test to identify a route between Altrincham and Piccadilly
    /// using their station names as params.
    /// This should return that an interchange is not required,
    /// with the expected Stop Objects.
    /// </summary>
    [Test]
    public void TestIdentifyAltrinchamPiccadillyNames()
    {
        var plannedRoute = _journeyPlannerModel?.PlanJourney("Altrincham", "Piccadilly");
        Assert.IsNotNull(plannedRoute);
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var piccadillyStop = _importedStops?.First(stop => stop.StopName == "Piccadilly");
        Assert.AreEqual(altrinchamStop, plannedRoute?.OriginStop);
        Assert.AreEqual(piccadillyStop, plannedRoute?.DestinationStop);
        Assert.IsFalse(plannedRoute?.RequiresInterchange);
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.AreEqual(1, plannedRoute?.RoutesFromOrigin.Count);
        Assert.AreEqual(purpleRoute, plannedRoute?.RoutesFromOrigin.First());
    }
}