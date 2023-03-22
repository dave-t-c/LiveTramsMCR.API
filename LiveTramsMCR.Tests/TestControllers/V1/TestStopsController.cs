using System.Collections.Generic;
using LiveTramsMCR.Controllers.V1;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ImportedResources = LiveTramsMCR.Tests.Resources.ResourceLoaders.ImportedResources;
using ResourcesConfig = LiveTramsMCR.Tests.Resources.ResourceLoaders.ResourcesConfig;

namespace LiveTramsMCR.Tests.TestControllers.V1;

/// <summary>
///     Test class for the StopsController class
/// </summary>
public class TestStopsController
{
    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private MockStopsRepository? _mockStopsRepository;
    private IStopsDataModel? _stopsDataModel;
    private StopsController? _testStopController;

    [SetUp]
    public void Setup()
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
        _mockStopsRepository =
            new MockStopsRepository(_importedResources.ImportedStops);
        _stopsDataModel = new StopsDataModel(_mockStopsRepository);
        _testStopController = new StopsController(_stopsDataModel);
    }

    /// <summary>
    ///     Reset the test Stop Controller to clear any changes made
    ///     during the tests.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        _testStopController = null;
        _resourcesConfig = null;
        _importedResources = null;
        _stopsDataModel = null;
        _testStopController = null;
    }

    /// <summary>
    ///     Test to try and get the expected 200 result code
    ///     from the GetAllStops method.
    /// </summary>
    [Test]
    public void TestGetAllStopsExpectedResultCode()
    {
        var result = _testStopController!.GetAllStops();
        Assert.IsNotNull(result);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult!.StatusCode);
    }

    /// <summary>
    ///     Test the length of the returned Stops list.
    ///     This should be 99.
    /// </summary>
    [Test]
    public void TestGetExpectedStopsCount()
    {
        var result = _testStopController!.GetAllStops();
        Assert.IsNotNull(result);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var retrievedStops = okResult!.Value as List<Stop> ?? new List<Stop>();
        Assert.AreEqual(99, retrievedStops.Count);
    }
}