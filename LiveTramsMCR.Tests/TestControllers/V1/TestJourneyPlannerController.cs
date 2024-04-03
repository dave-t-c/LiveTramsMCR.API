using System;
using System.Linq;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Controllers.V1;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestControllers.V1;

/// <summary>
///     Test class for the
/// </summary>
[TestFixture]
public class TestJourneyPlannerController : BaseNunitTest
{
    private ImportedResources? _importedResources;
    private IJourneyPlanner? _journeyPlanner;
    private JourneyPlannerController? _journeyPlannerController;
    private IJourneyPlannerModel? _journeyPlannerModel;
    private IRouteRepository? _routeRepository;
    private IStopsRepository? _stopsRepository;
    private ResourcesConfig? _resourcesConfig;

    [SetUp]
    public async Task SetUp()
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

        _stopsRepository = TestHelper.GetService<IStopsRepository>();
        MongoHelper.CreateRecords(AppConfiguration.StopsCollectionName, _importedResources.ImportedStops);
        await DynamoDbHelper.CreateRecords(_importedResources.ImportedStops);
        _routeRepository = TestHelper.GetService<IRouteRepository>();
        
        MongoHelper.CreateRecords(AppConfiguration.RoutesCollectionName, _importedResources.ImportedRoutes);
        await DynamoDbHelper.CreateRecords(_importedResources.ImportedRoutes);
        MongoHelper.CreateRecords(AppConfiguration.RouteTimesCollectionName, _importedResources.ImportedRouteTimes);
        await DynamoDbHelper.CreateRecords(_importedResources.ImportedRouteTimes);

        _journeyPlanner = new JourneyPlanner(_routeRepository);
        _journeyPlannerModel = new JourneyPlannerModel(_stopsRepository, _journeyPlanner);
        _journeyPlannerController = new JourneyPlannerController(_journeyPlannerModel);
    }

    [TearDown]
    public void TearDown()
    {
        _resourcesConfig = null;
        _importedResources = null;
        _journeyPlannerController = null;
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, null);
    }


    /// <summary>
    ///     Test to identify a planned journey between Altrincham and Piccadilly.
    ///     This should return an Ok result with no interchange being required,
    ///     with the expected start and end stops.
    /// </summary>
    [Test]
    public void TestHandleAltrinchamPiccadillyRoute()
    {
        var result = _journeyPlannerController?.PlanJourney("ALT", "PIC");
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
    
    [Test]
    public void TestDynamoDbJourneyPlanner()
    {
        MongoHelper.TearDownDatabase();
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, "true");
        var result = _journeyPlannerController?.PlanJourney("ALT", "PIC");
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
    ///     Test to plan a journey with an invalid origin name.
    ///     This should return a 400 bad request
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