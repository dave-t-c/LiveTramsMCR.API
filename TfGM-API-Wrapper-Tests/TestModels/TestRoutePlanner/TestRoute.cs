using System.Collections.Generic;
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
    private const string StopResourcePathConst = "../../../Resources/example-route-stops.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/routes.json";
    private ResourcesConfig? _validResourcesConfig;
    private List<Stop> _importedStops;
    private StopLoader _stopLoader;
    private Stop _exampleStop;
    
    
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
        _exampleStop = new Stop()
        {
            StopName = "Example"
        };

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
}