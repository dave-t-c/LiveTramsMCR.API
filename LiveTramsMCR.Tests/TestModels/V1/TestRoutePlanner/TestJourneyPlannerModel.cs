using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;
#pragma warning disable CS8604 // Possible null reference argument.

namespace LiveTramsMCR.Tests.TestModels.V1.TestRoutePlanner;

/// <summary>
///     Test class for the journey planner model.
/// </summary>
public class TestJourneyPlannerModel : BaseNunitTest
{
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private ImportedResources? _importedResources;
    private JourneyPlanner? _journeyPlanner;
    private JourneyPlannerModel? _journeyPlannerModel;
    private IRouteRepository? _routeRepository;
    private IStopsRepository? _stopsRepository;
    private ResourceLoader? _resourceLoader;
    private List<Route>? _routes;
    private ResourcesConfig? _validResourcesConfig;

    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesResourcePath,
            RouteTimesPath = RouteTimesPath
        };

        _resourceLoader = new ResourceLoader(_validResourcesConfig);
        _importedResources = _resourceLoader.ImportResources();

        _routeRepository = TestHelper.GetService<IRouteRepository>();
        _routes = _importedResources.ImportedRoutes;
        MongoHelper.CreateRecords(AppConfiguration.RoutesCollectionName, _importedResources?.ImportedRoutes);
        MongoHelper.CreateRecords(AppConfiguration.RouteTimesCollectionName, _importedResources?.ImportedRouteTimes);

        _stopsRepository = TestHelper.GetService<IStopsRepository>();
        MongoHelper.CreateRecords(AppConfiguration.StopsCollectionName, _importedResources?.ImportedStops);

        _journeyPlanner = new JourneyPlanner(_routeRepository);
        _journeyPlannerModel = new JourneyPlannerModel(_stopsRepository, _journeyPlanner);
    }

    [TearDown]
    public void TearDown()
    {
        _routes = null;
        _resourceLoader = null;
        _journeyPlanner = null;
        _importedResources = null;
        _validResourcesConfig = null;
    }

    /// <summary>
    ///     Test to identify a route between Altrincham and Piccadilly
    ///     using their station names as params.
    ///     This should return that an interchange is not required,
    ///     with the expected Stop Objects.
    /// </summary>
    [Test]
    public void TestIdentifyAltrinchamPiccadillyNames()
    {
        var plannedRoute = _journeyPlannerModel?.PlanJourney("Altrincham", "Piccadilly");
        Assert.IsNotNull(plannedRoute);
        var altrinchamStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Altrincham");
        var piccadillyStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Piccadilly");
        Assert.AreEqual(altrinchamStop, plannedRoute?.OriginStop);
        Assert.AreEqual(piccadillyStop, plannedRoute?.DestinationStop);
        Assert.IsFalse(plannedRoute?.RequiresInterchange);
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.AreEqual(1, plannedRoute?.RoutesFromOrigin.Count);
        Assert.AreEqual(purpleRoute?.Name, plannedRoute?.RoutesFromOrigin.First().Name);
    }

    /// <summary>
    ///     Test to identify a route between Altrincham and Ashton.
    ///     This should require an interchange and have a single route
    ///     from origin and from the interchange.
    /// </summary>
    [Test]
    public void TestIdentifyAltrinchamAshtonRoute()
    {
        var plannedRoute = _journeyPlannerModel?.PlanJourney("Altrincham", "Ashton-Under-Lyne");
        var altrinchamStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Altrincham");
        var ashtonStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Ashton-Under-Lyne");
        Assert.IsNotNull(plannedRoute);
        Assert.IsTrue(plannedRoute?.RequiresInterchange);
        Assert.AreEqual(altrinchamStop, plannedRoute?.OriginStop);
        Assert.AreEqual(ashtonStop, plannedRoute?.DestinationStop);
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.AreEqual(1, plannedRoute?.RoutesFromOrigin.Count);
        Assert.AreEqual(purpleRoute?.Name, plannedRoute?.RoutesFromOrigin.First().Name);
        var blueRoute = _routes?.First(route => route.Name == "Blue");
        Assert.AreEqual(1, plannedRoute?.RoutesFromInterchange.Count);
        Assert.AreEqual(blueRoute?.Name, plannedRoute?.RoutesFromInterchange.First().Name);
    }

    /// <summary>
    ///     Test to plan a journey with a null origin stop.
    ///     This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestJourneyPlannerModelNullOrigin()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _journeyPlannerModel?.PlanJourney(null, "Altrincham");
            });
    }

    /// <summary>
    ///     Test to identify a journey with a null destination.
    ///     This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestJourneyPlannerModelNullDestination()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _journeyPlannerModel?.PlanJourney("Altrincham", null);
            });
    }

    /// <summary>
    ///     Test to plan a journey with an invalid origin.
    ///     This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestJourneyPlannerModelInvalidOrigin()
    {
        Assert.Throws(Is.TypeOf<ArgumentException>(),
            delegate
            {
                var unused = _journeyPlannerModel?.PlanJourney("---", "Altrincham");
            });
    }

    /// <summary>
    ///     Test to plan a journey with an invalid destination.
    ///     This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestJourneyPlannerModelInvalidDestination()
    {
        Assert.Throws(Is.TypeOf<ArgumentException>(),
            delegate
            {
                var unused = _journeyPlannerModel?.PlanJourney("Altrincham", "---");
            });
    }
}