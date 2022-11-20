using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper_Tests.TestModels.TestRoutePlanner;

/// <summary>
/// Test class for the JourneyTimeFinder class 
/// </summary>
public class TestJourneyTimeFinder
{
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private ResourcesConfig? _validResourcesConfig;
    private StopLoader? _stopLoader;
    private List<Stop>? _importedStops;
    private RouteLoader? _routeLoader;
    private List<Route>? _routes;
    private RouteTimesLoader? _routeTimesLoader;
    private RouteTimes? _routeTimes;
    private JourneyTimeFinder? _journeyTimeFinder;

    /// <summary>
    /// Sets up required resources for tests
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesResourcePath,
            RouteTimesPath = RouteTimesPath
        };

        _stopLoader = new StopLoader(_validResourcesConfig);
        _importedStops = _stopLoader.ImportStops();

        _routeLoader = new RouteLoader(_validResourcesConfig, _importedStops);
        _routes = _routeLoader.ImportRoutes();

        _routeTimesLoader = new RouteTimesLoader(_validResourcesConfig);
        _routeTimes = _routeTimesLoader.ImportRouteTimes();

        _journeyTimeFinder = new JourneyTimeFinder(_routeTimes);
    }

    /// <summary>
    /// Clears created objects to avoid cross 
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _routes = null;
        _routeLoader = null;
        _importedStops = null;
        _stopLoader = null;
        _validResourcesConfig = null;
    }

    /// <summary>
    /// Test to identify the time for a journey between Deansgate and Cornbrook
    /// on the purple route.
    /// This should return 3 minutes.
    /// </summary>
    [Test]
    public void TestIdentifyTimeDeansgateCornbrookPurpleRoute()
    {
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        var deansgateStop = _importedStops?.First(stop => stop.StopName == "Deansgate - Castlefield");
        var cornbrookStop = _importedStops?.First(stop => stop.StopName == "Cornbrook");
        var result = _journeyTimeFinder?.FindJourneyTime(purpleRoute?.Name,
                deansgateStop?.StopName, cornbrookStop?.StopName);
        Assert.AreEqual(3, result);
    }

    /// <summary>
    /// Test to identify the time between Bury and Piccadilly on the
    /// yellow route.
    /// This should return 38 minutes.
    /// </summary>
    [Test]
    public void TestIdentifyTimeBuryPiccadillyYellowRoute()
    {
        var result = _journeyTimeFinder?.FindJourneyTime("Yellow",
            "Bury", "Piccadilly");
        Assert.AreEqual(38, result);
    }

    /// <summary>
    /// Test to find the route time between stops on a route that is invalid.
    /// This should throw an invalid operation exception
    /// </summary>
    [Test]
    public void TestIdentifyTimeInvalidRoute()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("The route 'Invalid' was not found"),
            delegate
            {
                var unused = _journeyTimeFinder?.FindJourneyTime("Invalid",
                    "Bury", "Piccadilly");
            });
    }

    /// <summary>
    /// Test to identify a route time with an origin stop that
    /// does not exist on the route.
    /// This should throw an invalid operation exception
    /// </summary>
    [Test]
    public void TestIdentifyTimeInvalidOrigin()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("The origin stop 'Invalid' was not found on the 'Yellow' route"),
            delegate
            {
                var unused = _journeyTimeFinder?.FindJourneyTime("Yellow",
                    "Invalid", "Piccadilly");
            });
    }

    /// <summary>
    /// Test to identify a route time with a destination stop
    /// that does not exist on the route.
    /// This should thrown an invalid operation exception.
    /// </summary>
    [Test]
    public void TestIdentifyTimeInvalidDestination()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("The destination stop 'Invalid' was not found on the 'Yellow' route"),
            delegate
            {
                var unused = _journeyTimeFinder?.FindJourneyTime("Yellow",
                    "Bury", "Invalid");
            });
    }
}