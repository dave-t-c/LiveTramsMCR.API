using System;
using System.Linq;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestStops;

/// <summary>
///     Test class for the StopLookup class
/// </summary>
public class TestStopLookup
{
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private ImportedResources? _importedResources;
    private MockStopsRepository? _mockStopsRepository;

    private ResourceLoader? _resourceLoader;
    private ResourcesConfig? _resourcesConfig;
    private StopLookup? _stopLookup;

    /// <summary>
    ///     Set up the required resources for each test.
    ///     This uses the resource loader to load the test resources.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _resourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutePath,
            RouteTimesPath = RouteTimesPath
        };

        _resourceLoader = new ResourceLoader(_resourcesConfig);
        _importedResources = _resourceLoader.ImportResources();
        _mockStopsRepository = new MockStopsRepository(_importedResources.ImportedStops);
        _stopLookup = new StopLookup(_mockStopsRepository);
    }

    /// <summary>
    ///     Reset the created objects to avoid problems between test runs.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _stopLookup = null;
        _importedResources = null;
        _resourceLoader = null;
        _resourcesConfig = null;
    }

    /// <summary>
    ///     Test to lookup the Altrincham Stop.
    ///     This should return the expected stop with a matching stop name.
    /// </summary>
    [Test]
    public void TestStopObjectLookupName()
    {
        var identifiedStop = _stopLookup?.LookupStop("Altrincham");
        var altrinchamStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Altrincham");
        Assert.IsNotNull(identifiedStop);
        Assert.AreEqual(altrinchamStop, identifiedStop);
    }

    /// <summary>
    ///     Test to lookup the Piccadilly stop based on its name.
    ///     This should return the expected stop object.
    /// </summary>
    [Test]
    public void TestStopObjectLookupDifferentName()
    {
        var identifiedStop = _stopLookup?.LookupStop("Piccadilly");
        var piccadillyStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Piccadilly");
        Assert.IsNotNull(identifiedStop);
        Assert.AreEqual(piccadillyStop, identifiedStop);
    }

    /// <summary>
    ///     Test to lookup a stop based on its tlaref.
    ///     This should return the Altrincham stop.
    /// </summary>
    [Test]
    public void TestStopObjectLookupTlaref()
    {
        var identifiedStop = _stopLookup?.LookupStop("ALT");
        var altrinchamStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Altrincham");
        Assert.IsNotNull(identifiedStop);
        Assert.AreEqual(identifiedStop, altrinchamStop);
    }

    /// <summary>
    ///     Test to lookup a stop with an invalid stop name.
    ///     This should throw an Invalid operation Exception.
    /// </summary>
    [Test]
    public void TestLookupStopObjectInvalidName()
    {
        Assert.Throws(Is.TypeOf<ArgumentException>()
                .And.Message.EqualTo("Value given is not a valid station name or TLAREF"),
            delegate { _stopLookup?.LookupStop("---"); });
    }

    /// <summary>
    ///     Test to lookup a stop object using a null value.
    ///     This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestLookupStopObjectNullValue()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'value')"),
            delegate { _stopLookup?.LookupStop(null); });
    }
}