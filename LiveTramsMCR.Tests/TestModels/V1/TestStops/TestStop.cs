using System.Linq;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestStops;

/// <summary>
/// Test class for the Stop object.
/// This tests the equals and hash code methods
/// </summary>
public class TestStop
{
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private ImportedResources? _importedResources;
    private Stop? _altrinchamStop;
    private Stop? _piccadillyStop;

    private ResourceLoader? _resourceLoader;
    private ResourcesConfig? _resourcesConfig;
    
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
        _altrinchamStop = _importedResources.ImportedStops.First(stop => stop.Tlaref == "ALT");
        _piccadillyStop = _importedResources.ImportedStops.First(stop => stop.Tlaref == "PIC");
        
    }

    [TearDown]
    public void TearDown()
    {
        _altrinchamStop = null;
        _importedResources = null;
        _resourceLoader = null;
        _resourcesConfig = null;
    }

    /// <summary>
    /// Test to check that two identical stops are equal.
    /// This should return true
    /// </summary>
    [Test]
    public void TestIdenticalReferencesEqual()
    {
        Assert.IsTrue(_altrinchamStop!.Equals(_altrinchamStop));
    }

    /// <summary>
    /// Test to see if different stops are unequal.
    /// This should return false
    /// </summary>
    [Test]
    public void TestDifferentStopsUnequal()
    {
        Assert.IsFalse(_altrinchamStop!.Equals(_piccadillyStop));
    }
    
    /// <summary>
    /// Test to check if a stop is equal to null.
    /// This should return false
    /// </summary>
    [Test]
    public void TestNullEquals()
    {
        Assert.IsFalse(_altrinchamStop!.Equals(null));
    }

    /// <summary>
    /// Test to get the hash code for the same stop twice.
    /// This should return the same value.
    /// </summary>
    [Test]
    public void TestGetHashCodeSameStop()
    {
        Assert.AreEqual(_altrinchamStop!.GetHashCode(), _altrinchamStop!.GetHashCode());
    }
    
    /// <summary>
    /// Test to get the hash code of different stops.
    /// These should be unequal
    /// </summary>
    [Test]
    public void TestGetHashCodeDifferentStops()
    {
        Assert.AreNotEqual(_altrinchamStop!.GetHashCode(), _piccadillyStop!.GetHashCode());
    }
}