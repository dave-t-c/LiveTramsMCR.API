using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V2.TestRoutePlanner;

/// <summary>
///     Test class for testing the route class.
///     This will use examples of routes and stops
/// </summary>
public class TestRouteV2
{
    private const string StopResourcePathConst = "../../../Resources/StopsV2.json";
    private const string RoutesResourcePath = "../../../Resources/RoutesV2.json";
    private StopKeysV2? _exampleAltrinchamStopKeys;
    private StopKeysV2? _exampleEastDidsburyStopKeysV2;
    private RouteV2? _exampleRoute;
    private List<RouteV2>? _importedRoutes;
    private List<StopV2>? _importedStops;
    private RouteV2Loader? _routeV2Loader;
    private StopV2Loader? _stopLoader;
    private ResourcesConfig? _validResourcesConfig;


    /// <summary>
    ///     Setup required stops for route tests
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopV2ResourcePath = StopResourcePathConst, RoutesV2ResourcePath = RoutesResourcePath
        };

        _stopLoader = new StopV2Loader(_validResourcesConfig);
        _importedStops = _stopLoader.ImportStops();

        _routeV2Loader = new RouteV2Loader(_validResourcesConfig);
        _importedRoutes = _routeV2Loader.ImportRoutes();

        var exampleEastDidsburyStopV2 = _importedStops.Single(stop => stop.Tlaref == "EDD");
        _exampleEastDidsburyStopKeysV2 = new StopKeysV2
        {
            StopName = exampleEastDidsburyStopV2.StopName, Tlaref = exampleEastDidsburyStopV2.Tlaref
        };

        _exampleRoute = _importedRoutes.Single(route => route.Name == "Purple");
        _exampleAltrinchamStopKeys = _exampleRoute.Stops.First(stop => stop.Tlaref == "ALT");
    }

    /// <summary>
    ///     Clear created routes to avoid cross test bugs
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
    }

    /// <summary>
    ///     Identify the stops that occur between two stops on a route.
    ///     This should return a list of one stop,
    ///     as the stops file contains Example-1, Example-2, Example 3.
    /// </summary>
    [Test]
    public void TestGetStopsBetweenOnRoute()
    {
        var identifiedStops = _exampleRoute?.GetIntermediateStops(
            _exampleRoute.Stops.First(),
            _exampleRoute.Stops.Last());
        var expectedStop = _importedStops?.First(stop => stop.StopName == "Navigation Road");
        Assert.IsNotEmpty(identifiedStops ?? new List<StopV2>());
        Assert.AreEqual(12, identifiedStops?.Count);
        Assert.IsTrue(identifiedStops?.Contains(expectedStop));
    }

    /// <summary>
    ///     Test to try and get the stops between a start and end location in a different order.
    ///     This should return two stops, in the order Example-3, Example-2.
    /// </summary>
    [Test]
    public void TestGetStopsBetweenBackwards()
    {
        var identifiedStops = _exampleRoute?.GetIntermediateStops(
            _exampleRoute?.Stops.Last(),
            _exampleRoute?.Stops.First());
        var firstExpectedStopKeys = _exampleRoute?.Stops.First(stop => stop.StopName == "Navigation Road");
        var secondExpectedStopKeys = _exampleRoute?.Stops.First(stop => stop.StopName == "Cornbrook");
        Assert.IsNotEmpty(identifiedStops ?? new List<StopV2>());
        Assert.AreEqual(12, identifiedStops?.Count);
        Assert.AreEqual(1, identifiedStops?.Count(stop => stop.StopName == firstExpectedStopKeys?.StopName));
        Assert.AreEqual(1, identifiedStops?.Count(stop => stop.StopName == secondExpectedStopKeys?.StopName));
        var firstExpectedStop = identifiedStops?.Single(stop => stop.StopName == firstExpectedStopKeys?.StopName);
        var secondExpectedStop = identifiedStops?.Single(stop => stop.StopName == secondExpectedStopKeys?.StopName);
        Assert.IsTrue(identifiedStops?.IndexOf(secondExpectedStop) < identifiedStops?.IndexOf(firstExpectedStop));
    }

    /// <summary>
    ///     Test to get the stops between a null start and end stop.
    ///     This should throw an arg null exception.
    /// </summary>
    [Test]
    public void TestRouteBetweenNullStart()
    {

        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _exampleRoute?.GetIntermediateStops(null, _exampleAltrinchamStopKeys);
            });
    }

    /// <summary>
    ///     Test to get the stops between a valid start and null stop.
    ///     This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestRouteBetweenNullEnd()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _exampleRoute?.GetIntermediateStops(_exampleEastDidsburyStopKeysV2, null);
            });
    }

    /// <summary>
    ///     Test to get the routes between a valid end stop, and a start index
    ///     not on the route.
    ///     This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestStartNotInRoute()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("East Didsbury does not exist on Purple route"),
            delegate
            {
                var unused = _exampleRoute?.GetIntermediateStops(
                    _exampleEastDidsburyStopKeysV2,
                    _exampleAltrinchamStopKeys);
            });
    }

    /// <summary>
    ///     Test to get the routes between a valid start stop, and an end stop
    ///     not on the route.
    ///     This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestEndNotInRoute()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("East Didsbury does not exist on Purple route"),
            delegate
            {
                var unused = _exampleRoute?.GetIntermediateStops(
                    _exampleAltrinchamStopKeys,
                    _exampleEastDidsburyStopKeysV2);
            });
    }
}