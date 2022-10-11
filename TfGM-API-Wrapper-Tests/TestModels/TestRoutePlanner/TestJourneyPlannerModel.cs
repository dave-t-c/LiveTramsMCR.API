using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        _routes = _importedResources?.ImportedRoutes;
        _journeyPlanner = new JourneyPlanner(_importedResources?.ImportedRoutes);
        _journeyPlannerModel = new JourneyPlannerModel(_importedResources, _journeyPlanner);
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
        var altrinchamStop = _importedResources?.ImportedStops?.First(stop => stop.StopName == "Altrincham");
        var piccadillyStop = _importedResources?.ImportedStops?.First(stop => stop.StopName == "Piccadilly");
        Assert.AreEqual(altrinchamStop, plannedRoute?.OriginStop);
        Assert.AreEqual(piccadillyStop, plannedRoute?.DestinationStop);
        Assert.IsFalse(plannedRoute?.RequiresInterchange);
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.AreEqual(1, plannedRoute?.RoutesFromOrigin.Count);
        Assert.AreEqual(purpleRoute, plannedRoute?.RoutesFromOrigin.First());
    }

    /// <summary>
    /// Test to identify a route between Altrincham and Ashton.
    /// This should require an interchange and have a single route
    /// from origin and from the interchange.
    /// </summary>
    [Test]
    public void TestIdentifyAltrinchamAshtonRoute()
    {
        var plannedRoute = _journeyPlannerModel?.PlanJourney("Altrincham", "Ashton-Under-Lyne");
        var altrinchamStop = _importedResources?.ImportedStops?.First(stop => stop.StopName == "Altrincham");
        var ashtonStop = _importedResources?.ImportedStops?.First(stop => stop.StopName == "Ashton-Under-Lyne");
        Assert.IsNotNull(plannedRoute);
        Assert.IsTrue(plannedRoute?.RequiresInterchange);
        Assert.AreEqual(altrinchamStop, plannedRoute?.OriginStop);
        Assert.AreEqual(ashtonStop, plannedRoute?.DestinationStop);
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.AreEqual(1, plannedRoute?.RoutesFromOrigin.Count);
        Assert.AreEqual(purpleRoute, plannedRoute?.RoutesFromOrigin.First());
        var blueRoute = _routes?.First(route => route.Name == "Blue");
        Assert.AreEqual(1, plannedRoute?.RoutesFromInterchange.Count);
        Assert.AreEqual(blueRoute, plannedRoute?.RoutesFromInterchange.First());
    }

    /// <summary>
    /// Test to plan a journey with a null origin stop.
    /// This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestJourneyPlannerModelNullOrigin()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _journeyPlannerModel?.PlanJourney( null, "Altrincham");
            });
    }
    
    /// <summary>
    /// Test to identify a journey with a null destination.
    /// This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestJourneyPlannerModelNullDestination()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _journeyPlannerModel?.PlanJourney( "Altrincham", null);
            });
    }
    
    /// <summary>
    /// Test to plan a journey with an invalid origin.
    /// This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestJourneyPlannerModelInvalidOrigin()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>(),
            delegate
            {
                var unused = _journeyPlannerModel?.PlanJourney("---", "Altrincham");
            });
    }
    
    /// <summary>
    /// Test to plan a journey with an invalid destination.
    /// This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestJourneyPlannerModelInvalidDestination()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>(),
            delegate
            {
                var unused = _journeyPlannerModel?.PlanJourney("Altrincham", "---");
            });
    }
}