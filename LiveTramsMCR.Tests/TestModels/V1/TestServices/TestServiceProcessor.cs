using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;
using ImportedResources = LiveTramsMCR.Tests.Resources.ResourceLoaders.ImportedResources;
using ResourcesConfig = LiveTramsMCR.Tests.Resources.ResourceLoaders.ResourcesConfig;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

/// <summary>
///     Test class for the ServiceProcessor.
/// </summary>
public class TestServiceProcessor
{
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesPath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private ImportedResources? _importedResources;
    private List<int> _bmrIds;
    private MockServiceRequester? _mockServiceRequester;

    private ResourceLoader? _resourceLoader;
    private ServiceProcessor? _serviceProcessor;
    private ResourcesConfig? _validResourcesConfig;
    private MockStopsRepository? _mockStopsRepository;

    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesPath,
            RouteTimesPath = RouteTimesPath
        };

        _resourceLoader = new ResourceLoader(_validResourcesConfig);
        _importedResources = _resourceLoader.ImportResources();

        _bmrIds = _importedResources.ImportedStops.First(stop => stop.Tlaref == "BMR").Ids;
        
        _mockServiceRequester = new MockServiceRequester(_bmrIds);

        _mockStopsRepository = new MockStopsRepository(_importedResources.ImportedStops);
        
        _serviceProcessor = new ServiceProcessor(_mockServiceRequester, _mockStopsRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _mockServiceRequester = null;
        _validResourcesConfig = null;
        _resourceLoader = null;
        _importedResources = null;
        _mockServiceRequester = null;
        _serviceProcessor = null;
    }


    /// <summary>
    ///     This should take a stop name, and then return the expected formatted services.
    ///     This will use tge MockServiceRequester so we can test an expected value as the data
    ///     will be static.
    ///     This should return FormattedServices.
    /// </summary>
    [Test]
    public void TestProcessServicesStopName()
    {
        Debug.Assert(_serviceProcessor != null, nameof(_serviceProcessor) + " != null");
        var result = _serviceProcessor.RequestServices("BMR");
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Destinations.Count);
    }

    /// <summary>
    ///     Test to use the service processor with a null stop name.
    ///     This should throw a null argument exception
    /// </summary>
    [Test]
    public void TestServiceProcessorNullName()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stop')"),
            delegate
            {
                Debug.Assert(_serviceProcessor != null, nameof(_serviceProcessor) + " != null");
                _serviceProcessor.RequestServices(null);
            });
    }

    /// <summary>
    /// Test to request services in the format for a departure board.
    /// </summary>
    [Test]
    public void TestServicesDepartureBoard()
    {
        var result = _serviceProcessor?.RequestDepartureBoardServices("BMR");
        Assert.IsNotNull(result);
        var trams = result?.Trams;
        Assert.IsNotNull(trams);
        Assert.AreEqual(3, trams?.Count);
        var firstTram = trams?.First();
        Assert.AreEqual("0", firstTram?.Wait);
        Assert.AreEqual(3, trams?.Distinct().Count());
        Assert.AreEqual("23", trams?.Last().Wait);
    }

    
    /// <summary>
    /// Test to try and request departure board services with a null stop name.
    /// This should throw an arg null exception.
    /// </summary>
    [Test]
    public void TestServicesDepartureBoardNullStop()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stop')"),
            delegate
            {
                _serviceProcessor?.RequestDepartureBoardServices(null);
            });
    }
}