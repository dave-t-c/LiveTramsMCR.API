using System;
using System.IO;
using LiveTramsMCR.Models.V1.Resources;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestResources;

public class TestRouteTimesLoader
{
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private ResourcesConfig? _validResourcesConfig;
    private ResourcesConfig? _invalidRouteTimesPathConfig;
    private RouteTimesLoader? _validRouteTimesLoader;
    private const int ExpectedStopsCount = 99;

    [SetUp]
    public void SetUp()
    {
        //The links for the resources folder is three directories up due to where
        //the tests are run from.
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesResourcePath,
            RouteTimesPath = RouteTimesPath
        };

        _validRouteTimesLoader = new RouteTimesLoader(_validResourcesConfig);

        _invalidRouteTimesPathConfig = _validResourcesConfig.DeepCopy();
        _invalidRouteTimesPathConfig.RouteTimesPath = "../../../Resources/NonExistentFile.json";

    }

    /// <summary>
    /// Test to import the route times from the resources file
    /// This should import the expected number of routes and stops.
    /// </summary>
    [Test]
    public void TestImportRouteTimes()
    {
        var routeTimesLoader = new RouteTimesLoader(_validResourcesConfig);
        var result = routeTimesLoader.ImportRouteTimes();
        Assert.NotNull(result);
        var allRoutes = result.GetAllRoutes();
        Assert.NotNull(allRoutes);
        Assert.AreEqual(8, allRoutes.Count);
    }

    /// <summary>
    /// Test to create a route times loader with a null resources config file.
    /// This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestImportRouteTimesNullConfig()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'resourcesConfig')"),
            delegate
            {
                var unused = new RouteTimesLoader(null);
            });
    }

    /// <summary>
    /// Test to create a route times object with an invalid route times
    /// file.
    /// This should throw a file not found exception.
    /// </summary>
    [Test]
    public void TestCreateRouteTimesInvalidRouteTimesResource()
    {
        Assert.Throws(Is.TypeOf<FileNotFoundException>()
                .And.Message.Contains("Could not find file")
                .And.Message.Contains("../../../Resources/NonExistentFile.json"),
            delegate
            {
                var routeTimesLoader = new RouteTimesLoader(_invalidRouteTimesPathConfig);
                routeTimesLoader.ImportRouteTimes();
            });
    }

}