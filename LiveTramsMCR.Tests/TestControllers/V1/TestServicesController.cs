using System.Linq;
using LiveTramsMCR.Controllers.V1;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using LiveTramsMCR.Tests.TestModels.V1.TestServices;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ImportedResources = LiveTramsMCR.Tests.Resources.ResourceLoaders.ImportedResources;
using ResourcesConfig = LiveTramsMCR.Tests.Resources.ResourceLoaders.ResourcesConfig;

namespace LiveTramsMCR.Tests.TestControllers.V1;

/// <summary>
/// Test class for the ServiceController class.
/// </summary>
public class TestServicesController
{
    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private IRequester? _requester;
    private MockStopsRepository? _mockStopsRepository;
    private IServicesDataModel? _servicesDataModel;
    private ServiceController? _serviceController;
    
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
        _requester = new MockServiceRequester();
        _mockStopsRepository = new MockStopsRepository(_importedResources.ImportedStops);
        _servicesDataModel = new ServicesDataModel(_mockStopsRepository, _requester);
        _serviceController = new ServiceController(_servicesDataModel);
    }

    [TearDown]
    public void TearDown()
    {
        _resourcesConfig = null;
        _importedResources = null;
        _requester = null;
        _servicesDataModel = null;
        _serviceController = null;
    }

    /// <summary>
    /// Test to request services for BMR.
    /// This should again return services  with a single destination.
    /// This should also return an OK status code
    /// </summary>
    [Test]
    public void TestRequestServices()
    {
        var result = _serviceController?.GetService("BMR");
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult!.StatusCode);
        var returnedServices = okResult.Value as FormattedServices;
        Assert.NotNull(returnedServices);
        Assert.AreEqual(1, returnedServices?.Destinations.Count);
    }

    /// <summary>
    /// Requests services for a non-existent stop name
    /// This should return a 400 bad request code.
    /// </summary>
    [Test]
    public void TestRequestServicesInvalidName()
    {
        var result = _serviceController?.GetService("AAA");
        Assert.NotNull(result);
        var requestObj = result as ObjectResult;
        Assert.NotNull(requestObj);
        Assert.AreEqual(400, requestObj?.StatusCode);
    }

    /// <summary>
    /// Request a service with a null stop name.
    /// This should return a 400 bad request
    /// </summary>
    [Test]
    public void TestRequestNullStopName()
    {
        var result = _serviceController?.GetService(null);
        Assert.NotNull(result);
        var requestObj = result as ObjectResult;
        Assert.NotNull(requestObj);
        Assert.AreEqual(400, requestObj?.StatusCode);
    }
    
    /// <summary>
    /// Test to request departure board services for BMR.
    /// This should return 3 trams
    /// This should also return an OK status code
    /// </summary>
    [Test]
    public void TestRequestDepartureBoardServices()
    {
        var result = _serviceController?.GetDepartureBoardService("BMR");
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult!.StatusCode);
        var returnedServices = okResult.Value as FormattedDepartureBoardServices;
        Assert.NotNull(returnedServices);
        var trams = returnedServices?.Trams;
        Assert.IsNotNull(trams);
        Assert.AreEqual(3, trams?.Count);
        Assert.AreEqual(3, trams?.Distinct().Count());
        var firstTram = trams?.First();
        Assert.AreEqual("0", firstTram?.Wait);
        var finalTram = trams?.Last();
        Assert.AreEqual("23", finalTram?.Wait);
    }

    /// <summary>
    /// Requests departure board services for a non-existent stop name
    /// This should return a 400 bad request code.
    /// </summary>
    [Test]
    public void TestRequestDepartureBoardServicesInvalidName()
    {
        var result = _serviceController?.GetDepartureBoardService("AAA");
        Assert.NotNull(result);
        var requestObj = result as ObjectResult;
        Assert.NotNull(requestObj);
        Assert.AreEqual(400, requestObj?.StatusCode);
    }

    /// <summary>
    /// Request departure board services with a null stop name.
    /// This should return a 400 bad request
    /// </summary>
    [Test]
    public void TestRequestDepartureBoardServicesNullStopName()
    {
        var result = _serviceController?.GetDepartureBoardService(null);
        Assert.NotNull(result);
        var requestObj = result as ObjectResult;
        Assert.NotNull(requestObj);
        Assert.AreEqual(400, requestObj?.StatusCode);
    }
}