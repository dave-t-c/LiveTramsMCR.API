
using System.Collections.Generic;
using LiveTramsMCR.Controllers.V2;
using LiveTramsMCR.Models.V2.RoutePlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestControllers.V2;

public class TestRoutesControllerV2
{

    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private IRouteRepositoryV2? _routeRepositoryV2;
    private IRoutesDataModelV2? _routesDataModel;
    private RoutesControllerV2? _routesControllerV2;

    [SetUp]
    public void Setup()
    {
        _resourcesConfig = new ResourcesConfig()
        {
            RoutesV2ResourcePath = "../../../Resources/RoutesV2.json"
        };
        _importedResources = new ResourceLoader(_resourcesConfig).ImportResources();
        _routeRepositoryV2 = new MockRouteRepositoryV2(_importedResources.ImportedRoutesV2);
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
    /// Test to get the expected result code when getting all routes.
    /// This should return a 200 OK.
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
    /// Test to get all routes from the mock stops repository.
    /// This should return the expected count of 8.
    /// </summary>
    [Test]
    public void TestGetAllRoutesExpectedCount()
    {
        var result = _routesControllerV2?.GetRoutes();
        Assert.IsNotNull(result);
        
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var retrievedStops = okResult!.Value as List<RouteV2> ?? new List<RouteV2>();
        Assert.AreEqual(8, retrievedStops.Count);
    }
    
    
}