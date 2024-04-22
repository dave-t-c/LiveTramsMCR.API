using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Controllers.V1;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestControllers.V1;

/// <summary>
///     Test class for the StopsController class
/// </summary>
public class TestStopsController : BaseNunitTest
{
    private ImportedResources? _importedResources;
    private IStopsRepository? _stopsRepository;
    private ResourcesConfig? _resourcesConfig;
    private IStopsDataModel? _stopsDataModel;
    private StopsController? _testStopController;

    [SetUp]
    public async Task Setup()
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
        _stopsRepository = TestHelper.GetService<IStopsRepository>();
        MongoHelper.CreateRecords(AppConfiguration.StopsCollectionName, _importedResources.ImportedStops);
        await DynamoDbTestHelper.CreateRecords(_importedResources.ImportedStops);
        _stopsDataModel = new StopsDataModel(_stopsRepository);
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
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, null);
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

    [Test]
    public void TestGetExpectedStopsDynamoDb()
    {
        MongoHelper.TearDownDatabase();
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, "true");
        var result = _testStopController!.GetAllStops();
        Assert.IsNotNull(result);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var retrievedStops = okResult!.Value as List<Stop> ?? new List<Stop>();
        Assert.AreEqual(99, retrievedStops.Count);
    }
}