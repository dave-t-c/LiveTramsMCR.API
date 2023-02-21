using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.Stops;
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
        _stopLookup = new StopLookup(_importedResources);
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
    ///     Test to look up the Tlaref 'ALT'
    ///     and check the IDs returned are an array of list of [728, 729]
    /// </summary>
    [Test]
    public void TestStopLookupTlarefIDs()
    {
        const string tlaref = "ALT";
        var expectedResult = new List<int> {728, 729};
        Debug.Assert(_stopLookup != null, nameof(_stopLookup) + " != null");
        var result = _stopLookup.TlarefLookup(tlaref);
        Assert.NotNull(result);
        Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    ///     Test to get IDs for a different tlaref.
    ///     This should return an array of length 4
    ///     instead of length 2.
    /// </summary>
    [Test]
    public void TestStopLookupDifferentTlaref()
    {
        const string tlaref = "ASH";
        var expectedResult = new List<int> {783, 784, 785, 786};
        Debug.Assert(_stopLookup != null, nameof(_stopLookup) + " != null");
        var result = _stopLookup.TlarefLookup(tlaref);
        Assert.NotNull(result);
        Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    ///     Test to try and get the tlaref for a null value.
    ///     This should throw an illegal argument exception
    /// </summary>
    [Test]
    public void TestStopLookupNullTlaref()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'tlaref')"),
            delegate { _stopLookup?.TlarefLookup(null); });
    }

    /// <summary>
    ///     Test to try and get the stops IDs from a given stop name.
    ///     This should return the expected IDs.
    /// </summary>
    [Test]
    public void TestStopLookupStopName()
    {
        const string stationName = "Altrincham";
        var expectedResult = new List<int> {728, 729};
        Debug.Assert(_stopLookup != null, nameof(_stopLookup) + " != null");
        var result = _stopLookup.StationNameLookup(stationName);
        Assert.NotNull(result);
        Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    ///     Test to try and retrieve the IDs for a different station name.
    ///     This should return 4 IDs instead of the existing 2.
    /// </summary>
    [Test]
    public void TestStopLookupDifferentStop()
    {
        const string stationName = "Ashton-Under-Lyne";
        var expectedResult = new List<int> {783, 784, 785, 786};
        Debug.Assert(_stopLookup != null, nameof(_stopLookup) + " != null");
        var result = _stopLookup.StationNameLookup(stationName);
        Assert.NotNull(result);
        Assert.AreEqual(expectedResult, result);
    }


    /// <summary>
    ///     Test to try and lookup a null stop name.
    ///     This should through a null argument exception.
    /// </summary>
    [Test]
    public void TestNullStopLookup()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stationName')"),
            delegate { _stopLookup?.StationNameLookup(null); });
    }

    /// <summary>
    ///     Test to use the lookup Ids method with a tlaref.
    ///     This should return the expected IDs of length 2.
    /// </summary>
    [Test]
    public void TestLookupIDsTlaref()
    {
        const string tlaref = "ALT";
        var expectedResult = new List<int> {728, 729};
        Debug.Assert(_stopLookup != null, nameof(_stopLookup) + " != null");
        var result = _stopLookup.LookupIDs(tlaref);
        Assert.NotNull(result);
        Assert.AreEqual(expectedResult, result);
    }


    /// <summary>
    ///     Test to lookup the IDs for a different tlaref.
    ///     This should return a different list of length 4.
    /// </summary>
    [Test]
    public void TestLookupIDsDifferentTlaref()
    {
        const string tlaref = "ASH";
        var expectedResult = new List<int> {783, 784, 785, 786};
        Debug.Assert(_stopLookup != null, nameof(_stopLookup) + " != null");
        var result = _stopLookup.LookupIDs(tlaref);
        Assert.NotNull(result);
        Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    ///     Test to look up a station name using the LookUpIDs method.
    ///     This should return the values for the station name.
    /// </summary>
    [Test]
    public void TestLookupIDsStationName()
    {
        const string stationName = "Altrincham";
        var expectedResult = new List<int> {728, 729};
        Debug.Assert(_stopLookup != null, nameof(_stopLookup) + " != null");
        var result = _stopLookup.LookupIDs(stationName);
        Assert.NotNull(result);
        Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    ///     Test to lookup the IDs for a different station name.
    ///     This should return a different ID list of length 4.
    /// </summary>
    [Test]
    public void TestLookupIDsDifferentStationName()
    {
        const string stationName = "Ashton-Under-Lyne";
        var expectedResult = new List<int> {783, 784, 785, 786};
        Debug.Assert(_stopLookup != null, nameof(_stopLookup) + " != null");
        var result = _stopLookup.LookupIDs(stationName);
        Assert.NotNull(result);
        Assert.AreEqual(expectedResult, result);
    }

    /// <summary>
    ///     Test to lookup a value that is not either a valid tlaref or station name.
    ///     This should throw an argument exception.
    /// </summary>
    [Test]
    public void TestLookupNonExistentValue()
    {
        Assert.Throws(Is.TypeOf<ArgumentException>()
                .And.Message.EqualTo("Value given is not a valid station name or TLAREF"),
            delegate { _stopLookup?.LookupIDs("Invalid"); });
    }

    /// <summary>
    ///     Test to lookup the IDs for the value null.
    ///     This should throw a argument null exception.
    /// </summary>
    [Test]
    public void TestLookupIDsNull()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'value')"),
            delegate { _stopLookup?.LookupIDs(null); });
    }

    /// <summary>
    /// Test to lookup the Altrincham Stop.
    /// This should return the expected stop with a matching stop name.
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
    /// Test to lookup the Piccadilly stop based on its name.
    /// This should return the expected stop object.
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
    /// Test to lookup a stop based on its tlaref.
    /// This should return the Altrincham stop.
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
    /// Test to lookup a stop with an invalid stop name.
    /// This should throw an Invalid operation Exception.
    /// </summary>
    [Test]
    public void TestLookupStopObjectInvalidName()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("Sequence contains no matching element"),
            delegate { _stopLookup?.LookupStop("---"); });
    }

    /// <summary>
    /// Test to lookup a stop object using a null value.
    /// This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestLookupStopObjectNullValue()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'value')"),
            delegate { _stopLookup?.LookupStop(null); });
    }
}