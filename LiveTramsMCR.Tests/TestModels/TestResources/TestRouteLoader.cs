using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.Resources;
using LiveTramsMCR.Models.Stops;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.TestResources;

/// <summary>
/// Tests the route loader class.
/// This class should import the unprocessed Routes, and then
/// add the stops for each route to create a list of the Route type.
/// </summary>
public class TestRouteLoader
{
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string RoutesDiffLengthResourcePath = "../../../Resources/TestRoutePlanner/routes-diff-length.json";
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private ResourcesConfig? _validResourcesConfig;
    private ResourcesConfig? _diffRoutesResourcesConfig;
    private ResourcesConfig? _nullRoutesPathResourcesConfig;
    private StopLoader? _stopLoader;
    private List<Stop>? _importedStops;
    
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

        _diffRoutesResourcesConfig = _validResourcesConfig.DeepCopy();
        _diffRoutesResourcesConfig.RoutesResourcePath = RoutesDiffLengthResourcePath;

        _nullRoutesPathResourcesConfig = _validResourcesConfig.DeepCopy();
        _nullRoutesPathResourcesConfig.RoutesResourcePath = null;

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

    /// <summary>
    /// Test to import a different number of routes.
    /// This should return 7 routes.
    /// </summary>
    [Test]
    public void TestRouteImportDifferentNumberOfRoutes()
    {
        var testRouteLoader = new RouteLoader(_diffRoutesResourcesConfig, _importedStops);
        var importedRoutes = testRouteLoader.ImportRoutes();
        Assert.NotNull(importedRoutes);
        Assert.IsNotEmpty(importedRoutes);
        Assert.AreEqual(7, importedRoutes.Count);
    }

    /// <summary>
    /// Test to see if the Purple route contains the
    /// Piccadilly stop.
    /// This should be the last stop on the list, and should only exist once.
    /// </summary>
    [Test]
    public void TestRouteContainsImportedStops()
    {
        var testRouteLoader = new RouteLoader(_validResourcesConfig, _importedStops);
        var importedRoutes = testRouteLoader.ImportRoutes();
        Assert.NotNull(importedRoutes);
        Assert.IsNotEmpty(importedRoutes);
        Assert.AreEqual(8, importedRoutes.Count);
        var piccadillyStop = _importedStops?.First(stop => stop.StopName == "Piccadilly");
        var purpleRoute = importedRoutes.First(route => route.Name == "Purple");
        Assert.Contains(piccadillyStop, purpleRoute.Stops);
        Assert.AreEqual(1, purpleRoute.Stops.FindAll(stop => stop.StopName == "Piccadilly").Count);
    }

    
    /// <summary>
    /// Create a route loader with a null resources config.
    /// This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestRouteLoaderNullResources()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'resourcesConfig')"),
            delegate
            {
                var unused = new RouteLoader(null, _importedStops);
            });
    }

    /// <summary>
    /// Test to create a route loader with a null stops list.
    /// This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestRouteLoaderNullImportedStops()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'importedStops')"),
            delegate
            {
                var unused = new RouteLoader(_validResourcesConfig, null);
            });
    }

    /// <summary>
    /// Create a route loader with a null resources route path.
    /// This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestRouteLoaderNullRoutesPath()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("RoutesResourcePath cannot be null"),
            delegate
            {
                var unused = new RouteLoader(_nullRoutesPathResourcesConfig, _importedStops);
            });
    }
}