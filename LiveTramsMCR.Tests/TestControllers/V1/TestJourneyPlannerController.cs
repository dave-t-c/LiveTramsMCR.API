using System.Linq;
using LiveTramsMCR.Controllers.V1;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ImportedResources = LiveTramsMCR.Tests.Resources.ResourceLoaders.ImportedResources;
using ResourcesConfig = LiveTramsMCR.Tests.Resources.ResourceLoaders.ResourcesConfig;

namespace LiveTramsMCR.Tests.TestControllers.V1;

/// <summary>
/// Test class for the 
/// </summary>
public class TestJourneyPlannerController
{
    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private IJourneyPlanner? _journeyPlanner;
    private MockStopsRepository? _mockStopsRepository;
    private MockRouteRepository? _mockRouteRepository;
    private IJourneyPlannerModel? _journeyPlannerModel;
    private JourneyPlannerController? _journeyPlannerController;
    
    [SetUp]
    public void SetUp()
    {
        _resourcesConfig = new ResourcesConfig
        {
            StopResourcePath = "../../../Resources/TestRoutePlanner/stops.json",
            StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json",
            TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json",
            RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json",
            RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json"
        };
        _importedResources = new ResourceLoader(_resourcesConfig).ImportResources();
        _mockStopsRepository = new MockStopsRepository(_importedResources.ImportedStops);
        _mockRouteRepository =
            new MockRouteRepository(_importedResources.ImportedRoutes, _importedResources.ImportedRouteTimes);
        _journeyPlanner = new JourneyPlanner(_mockRouteRepository);
        _journeyPlannerModel = new JourneyPlannerModel(_mockStopsRepository, _journeyPlanner);
        _journeyPlannerController = new JourneyPlannerController(_journeyPlannerModel);
    }
    
    [TearDown]
    public void TearDown()
    {
        _resourcesConfig = null;
        _importedResources = null;
        _journeyPlannerController = null;
    }

    
    /// <summary>
    /// Test to identify a planned journey between Altrincham and Piccadilly.
    /// This should return an Ok result with no interchange being required,
    /// with the expected start and end stops.
    /// </summary>
    [Test]
    public void TestHandleAltrinchamPiccadillyRoute()
    {
        var result = _journeyPlannerController?.PlanJourney("Altrincham", "Piccadilly");
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.AreEqual(200, okResult?.StatusCode);
        var plannedJourney = okResult?.Value as PlannedJourney;
        Assert.NotNull(plannedJourney);
        Assert.IsFalse(plannedJourney?.RequiresInterchange);
        var altrinchamStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Altrincham");
        var piccadillyStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Piccadilly");
        Assert.AreEqual(altrinchamStop, plannedJourney?.OriginStop);
        Assert.AreEqual(piccadillyStop, plannedJourney?.DestinationStop);
    }

    /// <summary>
    /// Test to plan a journey with an invalid origin name.
    /// This should return a 400 bad request
    /// </summary>
    [Test]
    public void TestPlanJourneyInvalidOriginName()
    {
        var result = _journeyPlannerController?.PlanJourney("AAAA", "Piccadilly");
        Assert.NotNull(result);
        var resultObj = result as ObjectResult;
        Assert.NotNull(resultObj);
        Assert.AreEqual(400, resultObj?.StatusCode);
    }
}