using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper_Tests.TestModels.TestRoutePlanner;

/// <summary>
/// Test class for the RouteIdentifier.
/// </summary>
public class TestRouteIdentifier
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
    private RouteIdentifier? _routeIdentifier;
    
    /// <summary>
    /// Import routes / stops before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        //To make it easier to use different Stops in tests,
        //Stops are imported and then selected instead of created.
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

        _routeIdentifier = new RouteIdentifier(_routes);
    }

    /// <summary>
    /// Set stops and routes to null
    /// to ensure no cross-test contamination.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
        _importedStops = null;
        _stopLoader = null;
        _routes = null;
        _routeLoader = null;
        _routeIdentifier = null;
    }

    /// <summary>
    /// Test to determine if an interchange is required
    /// for two stops on the same route.
    /// This should return false.
    /// </summary>
    [Test]
    public void TestIsInterchangeRequired()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var saleStop = _importedStops?.First(stop => stop.StopName == "Sale");
        Assert.IsFalse(_routeIdentifier?.IsInterchangeRequired(altrinchamStop, saleStop));
    }
}