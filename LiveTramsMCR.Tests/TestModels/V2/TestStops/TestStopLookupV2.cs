using System.Collections.Generic;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V2.TestStops;

/// <summary>
/// Test class for StopLookupV2
/// </summary>
public class TestStopLookupV2
{
    private const string StopsV2ResourcePathConst = "../../../Resources/StopsV2.json";
    private List<StopV2>? _importedStops;
    private IStopsRepositoryV2? _mockStopsRepository;
    private StopLookupV2? _stopLookupV2;
    
    [SetUp]
    public void SetUp()
    {
        var resourcesConfig = new ResourcesConfig()
        {
            StopV2ResourcePath = StopsV2ResourcePathConst
        };
        var stopLoaderV2 = new StopV2Loader(resourcesConfig);
        _importedStops = stopLoaderV2.ImportStops();
        _mockStopsRepository = new MockStopsRepositoryV2(_importedStops);
        _stopLookupV2 = new StopLookupV2(_mockStopsRepository);
    }

    /// <summary>
    /// Test to find a valid stop.
    /// This should return the valid stop keys.
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
}