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
    }

    [TearDown]
    public void TearDown()
    {
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
}