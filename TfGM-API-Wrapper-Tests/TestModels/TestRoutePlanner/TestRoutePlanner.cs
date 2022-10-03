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
    /// This should return a planned route POCO with the expected origin and destination stops
    /// </summary>
    [Test]
    public void TestIdentifyRouteOnSameLine()
    {
        Assert.Pass();
        //This test case has already started being developed, but will 
        //need to wait until other sections of the route planning have been developed.
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var saleStop = _importedStops?.First(stop => stop.StopName == "Sale");
        var routePlanner = new RoutePlanner(_routes);
        var plannedRoutes = routePlanner.FindRoute(altrinchamStop, saleStop);
        //Should be two possible routes, one using purple, one using green.
        Assert.IsNotNull(plannedRoutes);
        Assert.IsNotEmpty(plannedRoutes);
        Assert.AreEqual(2, plannedRoutes.Count);
        var firstRoute = plannedRoutes[0];
        var secondRoute = plannedRoutes[1];
        Assert.IsNotNull(firstRoute);
        Assert.IsNotNull(secondRoute);
        Assert.AreEqual(altrinchamStop, firstRoute.OriginStop);
        Assert.AreEqual(altrinchamStop, secondRoute.DestinationStop);
    }
}