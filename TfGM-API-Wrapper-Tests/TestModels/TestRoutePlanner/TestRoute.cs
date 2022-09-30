using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper_Tests.TestModels.TestRoutePlanner;

/// <summary>
/// Test class for testing the route class.
/// This will use examples of routes and stops
/// </summary>
public class TestRoute
{
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/example-route-stops.json";
    private const string StopsResourcePathExtended = "../../../Resources/TestRoutePlanner/example-route-stops-extended.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/routes.json";
    private ResourcesConfig? _validResourcesConfig;
    private List<Stop>? _importedStops;
    private List<Stop>? _extendedImportedStops;
    private StopLoader? _stopLoader;
    private Stop? _exampleStop;
    private Route? _validRoute;
    private Route? _extendedStopsRoute;
    
    
    /// <summary>
    /// Setup required stops for route tests
    /// </summary>
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

        _stopLoader = new StopLoader(_validResourcesConfig);
        _importedStops = _stopLoader.ImportStops();

        _validResourcesConfig.StopResourcePath = StopsResourcePathExtended;
        _stopLoader = new StopLoader(_validResourcesConfig);
        _extendedImportedStops = _stopLoader.ImportStops();
        
        _exampleStop = new Stop()
        {
            StopName = "Example"
        };

        _validRoute = new Route("Example route", "#0044cc", _importedStops);
        _extendedStopsRoute = new Route("Example route", "#0044cc", _extendedImportedStops);

    }

    /// <summary>
    /// Clear created routes to avoid cross test bugs
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
    }

    /// <summary>
    /// Create a basic route with a single stop.
    /// This should create a route with a single stop, with matching name and colour.
    /// </summary>
    [Test]
    public void TestCreateBasicRoute()
    {
        var testRoute = new Route("Example", "#0044cc", new List<Stop> {new ()});
        Assert.AreEqual(1, testRoute.Stops.Count);
        Assert.AreEqual("Example", testRoute.Name);
        Assert.AreEqual("#0044cc", testRoute.Colour);
    }

    /// <summary>
    /// Test to create a test route with different values.
    /// </summary>
    [Test]
    public void TestCreateRouteWithDifferentValues()
    {
        var testRoute = new Route("Example-2", "#0044cd", new List<Stop> {_exampleStop});
        Assert.AreEqual(1, testRoute.Stops.Count);
        Assert.IsTrue(testRoute.Stops.Contains(_exampleStop));
        Assert.AreEqual("Example-2", testRoute.Name);
        Assert.AreEqual("#0044cd", testRoute.Colour);
    }

    /// <summary>
    /// Create a route with a null name.
    /// This should throw a args null exception.
    /// </summary>
    [Test]
    public void TestNullRouteName()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'name')"),
            delegate
            {
                var unused = new Route(null, "#0044cd", new List<Stop> {_exampleStop});
            });
    }

    /// <summary>
    /// Test creating a route with a null colour.
    /// This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestNullColour()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'colour')"),
            delegate
            {
                var unused = new Route("Example", null, new List<Stop> {_exampleStop});
            });
    }

    /// <summary>
    /// Test to create a route with a null stops list.
    /// This should throw an args null exception
    /// </summary>
    [Test]
    public void TestNullStops()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stops')"),
            delegate
            {
                var unused = new Route("Example", "#0044cd", null);
            });
    }

    /// <summary>
    /// Identify the stops that occur between two stops on a route.
    /// This should return a list of one stop,
    /// as the stops file contains Example-1, Example-2, Example 3.
    /// </summary>
    [Test]
    public void TestGetStopsBetweenOnRoute()
    {
        var identifiedStops = _validRoute?.GetStopsBetween(_importedStops?.First(), _importedStops?.Last());
        var expectedStop = _importedStops?.First(stop => stop.StopName == "Example-2");
        Assert.IsNotEmpty(identifiedStops ?? throw new NullReferenceException());
        Assert.AreEqual(1, identifiedStops.Count);
        Assert.IsTrue(identifiedStops.Contains(expectedStop));
    }

    /// <summary>
    /// Test to try and get the stops between a start and end location in a different order.
    /// This should return two stops, in the order Example-3, Example-2.
    /// </summary>
    [Test]
    public void TestGetStopsBetweenBackwards()
    {
        var identifiedStops =
            _extendedStopsRoute?.GetStopsBetween(_extendedImportedStops?.Last(), _extendedImportedStops?.First());
        var firstExpectedStop = _extendedImportedStops?.First(stop => stop.StopName == "Example-3");
        var secondExpectedStop = _extendedImportedStops?.First(stop => stop.StopName == "Example-2");
        Assert.IsNotEmpty(identifiedStops ?? throw new NullReferenceException());
        Assert.AreEqual(2, identifiedStops.Count);
        Assert.IsTrue(identifiedStops.Contains(firstExpectedStop));
        Assert.IsTrue(identifiedStops.Contains(secondExpectedStop));
        Assert.IsTrue(identifiedStops.IndexOf(firstExpectedStop) < identifiedStops.IndexOf(secondExpectedStop));
    }

    /// <summary>
    /// Test to get the stops between a null start and end stop.
    /// This should throw an arg null exception.
    /// </summary>
    [Test]
    public void TestRouteBetweenNullStart()
    {
        
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'start')"),
            delegate
            {
                var unused = _validRoute?.GetStopsBetween(null, _importedStops?.Last());
            });
    }

    /// <summary>
    /// Test to get the stops between a valid start and null stop.
    /// This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestRouteBetweenNullEnd()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'end')"),
            delegate
            {
                var unused = _validRoute?.GetStopsBetween(_importedStops?.First(), null);
            });
    }
}