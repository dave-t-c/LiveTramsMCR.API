using System;
using System.Collections.Generic;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V2.TestStops;

/// <summary>
///     Test class for StopLookupV2
/// </summary>
public class TestStopLookupV2 : BaseNunitTest
{
    private const string StopsV2ResourcePathConst = "../../../Resources/StopsV2.json";
    private List<StopV2>? _importedStops;
    private IStopsRepositoryV2? _stopsRepositoryV2;
    private StopLookupV2? _stopLookupV2;

    [SetUp]
    public void SetUp()
    {
        var resourcesConfig = new ResourcesConfig
        {
            StopV2ResourcePath = StopsV2ResourcePathConst
        };
        var stopLoaderV2 = new StopV2Loader(resourcesConfig);
        _importedStops = stopLoaderV2.ImportStops();
        _stopsRepositoryV2 = TestHelper.GetService<IStopsRepositoryV2>();
        MongoHelper.CreateRecords(AppConfiguration.StopsV2CollectionName, _importedStops);
        _stopLookupV2 = new StopLookupV2(_stopsRepositoryV2);
    }

    /// <summary>
    ///     Test to find a valid stop.
    ///     This should return the valid stop keys.
    /// </summary>
    [Test]
    public void TestLookupValidStop()
    {
        var returnedStop = _stopLookupV2?.LookupStop("ALT");
        Assert.IsNotNull(returnedStop);
        Assert.AreEqual("ALT", returnedStop?.Tlaref);
        Assert.AreEqual("Altrincham", returnedStop?.StopName);
    }

    [Test]
    public void TestLookupDifferentStop()
    {
        var returnedStop = _stopLookupV2?.LookupStop("EDD");
        Assert.IsNotNull(returnedStop);
        Assert.AreEqual("EDD", returnedStop?.Tlaref);
        Assert.AreEqual("East Didsbury", returnedStop?.StopName);
    }

    [Test]
    public void TestLookupNullStop()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'value')"),
            delegate { _stopLookupV2?.LookupStop(null); });
    }

    [Test]
    public void TestInvalidStopLookup()
    {
        Assert.Throws(Is.TypeOf<ArgumentException>()
                .And.Message.EqualTo("Value given is not a valid station name or TLAREF"),
            delegate { _stopLookupV2?.LookupStop("---"); });
    }
}