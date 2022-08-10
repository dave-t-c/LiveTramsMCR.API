using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TfGM_API_Wrapper_Tests.TestModels.TestServices;
using TfGM_API_Wrapper.Controllers;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.Services;

namespace TfGM_API_Wrapper_Tests.TestControllers;

/// <summary>
/// Test class for the ServiceController class.
/// </summary>
public class TestServicesController
{
    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private IRequester? _requester;
    private IServicesDataModel? _servicesDataModel;
    private ServiceController? _serviceController;
    
    [SetUp]
    public void SetUp()
    {
        _resourcesConfig = new ResourcesConfig
        {
            StopResourcePath = "../../../Resources/ValidStopLoader.json",
            StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json",
            TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json"
        };
        _importedResources = new ResourceLoader(_resourcesConfig).ImportResources();
        _requester = new MockServiceRequester();
        _servicesDataModel = new ServicesDataModel(_importedResources, _requester);
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
}