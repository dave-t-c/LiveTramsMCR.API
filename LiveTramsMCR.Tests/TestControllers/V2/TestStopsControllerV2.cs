using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Controllers.V2;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestControllers.V2;

/// <summary>
///     Test class for the StopsControllerV2 class
/// </summary>
public class TestStopsControllerV2 : BaseNunitTest
{
    private ImportedResources? _importedResources;
    private IStopsRepositoryV2? _stopsRepositoryV2;
    private ResourcesConfig? _resourcesConfig;
    private IStopsDataModelV2? _stopsDataModelV2;
    private StopsControllerV2? _testStopController;

    [SetUp]
    public async Task Setup()
    {
        _resourcesConfig = new ResourcesConfig
        {
            StopResourcePath = "../../../Resources/TestRoutePlanner/stops.json",
            StopV2ResourcePath = "../../../Resources/StopsV2.json",
            StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json",
            TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json",
            RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json",
            RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json"
        };
        _importedResources = new ResourceLoader(_resourcesConfig).ImportResources();
        _stopsRepositoryV2 = TestHelper.GetService<IStopsRepositoryV2>();
        MongoHelper.CreateRecords(AppConfiguration.StopsV2CollectionName, _importedResources.ImportedStopsV2);
        await DynamoDbHelper.CreateRecords(_importedResources.ImportedStopsV2);
        _stopsDataModelV2 = new StopsDataModelV2(_stopsRepositoryV2);
        _testStopController = new StopsControllerV2(_stopsDataModelV2);
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
        _stopsDataModelV2 = null;
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
        var retrievedStops = okResult!.Value as List<StopV2> ?? new List<StopV2>();
        Assert.AreEqual(99, retrievedStops.Count);
    }
    
    [Test]
    public void TestGetExpectedStopsCountDynamoDb()
    {
        MongoHelper.TearDownDatabase();
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, "true");
        var result = _testStopController!.GetAllStops();
        Assert.IsNotNull(result);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var retrievedStops = okResult!.Value as List<StopV2> ?? new List<StopV2>();
        Assert.AreEqual(99, retrievedStops.Count);
    }
}