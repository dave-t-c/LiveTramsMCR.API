using System.Collections.Generic;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper_Tests.TestModels.TestResources;

/// <summary>
/// Tests the route loader class.
/// This class should import the unprocessed Routes, and then
/// add the stops for each route to create a list of the Route type.
/// </summary>
public class TestRouteLoader
{
    private const string StopResourcePathConst = "../../../Resources/ValidStopLoader.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/routes.json";
    private ResourcesConfig? _validResourcesConfig;
    private StopLoader _stopLoader;
    private List<Stop> _importedStops;
    
    /// <summary>
    /// Create configs and stops for testing the route loader
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesResourcePath
        };

        _stopLoader = new StopLoader(_validResourcesConfig);
        _importedStops = _stopLoader.ImportStops();

    }

    /// <summary>
    /// Clear created configs to ensure no cross test contamination
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
    }

    /// <summary>
    /// Test to use a valid route import.
    /// This should return that 9 routes have been imported
    /// </summary>
    [Test]
    public void TestValidRouteImport()
    {
        var testRouteLoader = new RouteLoader(_validResourcesConfig, _importedStops);
        Assert.AreEqual(8, testRouteLoader.ImportRoutes().Count);
    }
}