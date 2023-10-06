using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Controllers.V2;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestControllers.V2;

public class TestRoutesControllerV2
{
    private ImportedResources? _importedResources;

    private ResourcesConfig? _resourcesConfig;
    private IRouteRepositoryV2? _routeRepositoryV2;
    private RoutesControllerV2? _routesControllerV2;
    private IRoutesDataModelV2? _routesDataModel;
    private IStopsRepositoryV2? _stopsRepositoryV2;

    [SetUp]
    public void Setup()
    {
        _resourcesConfig = new ResourcesConfig
        {
            RoutesV2ResourcePath = "../../../Resources/RoutesV2.json", StopV2ResourcePath = "../../../Resources/StopsV2.json"
        };
        _importedResources = new ResourceLoader(_resourcesConfig).ImportResources();
        _stopsRepositoryV2 = new MockStopsRepositoryV2(_importedResources.ImportedStopsV2);
        _routeRepositoryV2 = new MockRouteRepositoryV2(_importedResources.ImportedRoutesV2, _stopsRepositoryV2);
        _routesDataModel = new RoutesDataModelV2(_routeRepositoryV2);
        _routesControllerV2 = new RoutesControllerV2(_routesDataModel);
    }

    [TearDown]
    public void TearDown()
    {
        _routesControllerV2 = null;
        _routesDataModel = null;
        _routeRepositoryV2 = null;
        _importedResources = null;
        _resourcesConfig = null;
    }

    /// <summary>
    ///     Test to get the expected result code when getting all routes.
    ///     This should return a 200 OK.
    /// </summary>
    [Test]
    public void TestGetAllRoutesExpectedResultCode()
    {
        var result = _routesControllerV2?.GetRoutes();
        Assert.IsNotNull(result);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult?.StatusCode);
    }

    /// <summary>
    ///     Test to get all routes from the mock stops repository.
    ///     This should return the expected count of 8.
    /// </summary>
    [Test]
    public void TestGetAllRoutesExpectedCount()
    {
        var result = _routesControllerV2?.GetRoutes();
        Assert.IsNotNull(result);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var routes = okResult!.Value as List<RouteV2> ?? new List<RouteV2>();
        Assert.AreEqual(8, routes.Count);
    }

    /// <summary>
    ///     Test to get all routes and check the order of the stop details.
    ///     This should match the order of the stop keys list by checking the Tlaref of both
    /// </summary>
    [Test]
    public void TestGetAllRoutesExpectedRouteStops()
    {
        var result = _routesControllerV2?.GetRoutes();
        Assert.IsNotNull(result);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var routes = okResult!.Value as List<RouteV2> ?? new List<RouteV2>();
        Assert.AreEqual(8, routes.Count);

        var purpleRoute = routes.First(r => r.Name == "Purple");
        Assert.AreEqual(14, purpleRoute.Stops.Count);
        Assert.AreEqual(14, purpleRoute.StopsDetail?.Count);

        var stopsTlarefs = purpleRoute.Stops.Select(s => s.Tlaref);
        var stopsDetailTlarefs = purpleRoute.StopsDetail?.Select(s => s.Tlaref);

        CollectionAssert.AreEqual(stopsTlarefs, stopsDetailTlarefs);
    }
}