using System;
using System.Linq;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.RoutePlanner;

namespace TfGM_API_Wrapper_Tests.TestModels.TestResources;

public class TestRouteTimesLoader
{
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private ResourcesConfig? _validResourcesConfig;
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

}