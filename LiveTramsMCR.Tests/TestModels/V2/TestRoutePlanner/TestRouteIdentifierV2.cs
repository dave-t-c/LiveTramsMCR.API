using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Extensions;
using LiveTramsMCR.Tests.Helpers;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V2.TestRoutePlanner;

/// <summary>
///     Test class for the RouteIdentifier.
/// </summary>
public class TestRouteIdentifierV2 : BaseNunitTest
{
    private const string RoutesResourcePath = "../../../Resources/RoutesV2.json";
    private const string StopResourcePathConst = "../../../Resources/StopsV2.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private List<StopV2>? _importedStops;
    private MockRouteRepositoryV2? _mockRouteRepositoryV2;
    private IStopsRepositoryV2? _stopsRepositoryV2;
    private RouteIdentifierV2? _routeIdentifier;
    private List<RouteV2>? _routes;
    private RouteV2Loader? _routeV2Loader;
    private StopV2Loader? _stopV2Loader;
    private ResourcesConfig? _validResourcesConfig;

    /// <summary>
    ///     Import routes / stops before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        //To make it easier to use different Stops in tests,
        //Stops are imported and then selected instead of created.
        _validResourcesConfig = new ResourcesConfig
        {
            RouteTimesPath = RouteTimesPath, RoutesV2ResourcePath = RoutesResourcePath, StopV2ResourcePath = StopResourcePathConst
        };

        _stopV2Loader = new StopV2Loader(_validResourcesConfig);
        _importedStops = _stopV2Loader.ImportStops();
        _stopsRepositoryV2 = TestHelper.GetService<IStopsRepositoryV2>();
        MongoHelper.CreateRecords(AppConfiguration.StopsV2CollectionName, _importedStops);

        _routeV2Loader = new RouteV2Loader(_validResourcesConfig);
        _routes = _routeV2Loader.ImportRoutes();

        _mockRouteRepositoryV2 = new MockRouteRepositoryV2(
            _routes,
            _stopsRepositoryV2);

        _routeIdentifier = new RouteIdentifierV2(_mockRouteRepositoryV2);
    }

    /// <summary>
    ///     Set stops and routes to null
    ///     to ensure no cross-test contamination.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
        _importedStops = null;
        _routes = null;
        _routeIdentifier = null;
    }

    /// <summary>
    ///     Test to determine if an interchange is required
    ///     for two stops on the same route.
    ///     This should return false.
    /// </summary>
    [Test]
    public void TestIsInterchangeRequired()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham")
            .ConvertToStopKeysV2();

        var saleStop = _importedStops?.First(stop => stop.StopName == "Sale")
            .ConvertToStopKeysV2();

        Assert.IsFalse(_routeIdentifier?.IsInterchangeRequired(altrinchamStop, saleStop));
    }

    /// <summary>
    ///     Test to see if an interchange is required by checking stops on different routes.
    ///     This should return true
    /// </summary>
    [Test]
    public void TestInterchangeShouldBeRequired()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham")
            .ConvertToStopKeysV2();

        var ashtonStop = _importedStops?.First(stop => stop.StopName == "Ashton Moss")
            .ConvertToStopKeysV2();

        Assert.IsTrue(_routeIdentifier?.IsInterchangeRequired(altrinchamStop, ashtonStop));
    }

    /// <summary>
    ///     Test to determine if a null origin stop
    ///     requires an interchange to a valid dest.
    ///     This should throw an illegal args exception
    /// </summary>
    [Test]
    public void TestInterchangeNullOrigin()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham")
            .ConvertToStopKeysV2();
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _routeIdentifier?.IsInterchangeRequired(null, altrinchamStop);
            });
    }

    /// <summary>
    ///     Test to determine if an interchange is required between a valid origin stop
    ///     and a null dest.
    ///     This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestInterchangeNullDest()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham")
            .ConvertToStopKeysV2();

        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _routeIdentifier?.IsInterchangeRequired(altrinchamStop, null);
            });
    }

    /// <summary>
    ///     Test to identify an interchange stop for a journey where an interchange is required.
    ///     This should identify the stop shared on both routes that is closest to the destination.
    /// </summary>
    [Test]
    public void TestIdentifyInterChange()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham")
            .ConvertToStopKeysV2();

        var ashtonStop = _importedStops?.First(stop => stop.StopName == "Ashton Moss")
            .ConvertToStopKeysV2();

        var interchangeStop = _routeIdentifier?.IdentifyInterchangeStop(altrinchamStop, ashtonStop);
        Assert.NotNull(interchangeStop);
        Assert.AreEqual("Piccadilly", interchangeStop?.StopName);
    }


    /// <summary>
    ///     Test to identify the interchange between Eccles and the Trafford Centre.
    ///     This should return Pomona.
    /// </summary>
    [Test]
    public void TestIdentifyEcclesTraffordCentreInterchange()
    {
        var ecclesStop = _importedStops?.First(stop => stop.StopName == "Eccles")
            .ConvertToStopKeysV2();
        var traffordStop = _importedStops?.First(stop => stop.StopName == "The Trafford Centre")
            .ConvertToStopKeysV2();
        var interchangeStop = _routeIdentifier?.IdentifyInterchangeStop(ecclesStop, traffordStop);
        Assert.IsNotNull(interchangeStop);
        Assert.AreEqual("Pomona", interchangeStop?.StopName);
    }

    /// <summary>
    ///     Test to identify the interchange between the Airport and Bury.
    ///     This should return Victoria.
    /// </summary>
    [Test]
    public void TestIdentifyInterchangeAirportBury()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport")
            .ConvertToStopKeysV2();
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury")
            .ConvertToStopKeysV2();
        var interchangeStop = _routeIdentifier?.IdentifyInterchangeStop(airportStop, buryStop);
        Assert.IsNotNull(interchangeStop);
        Assert.AreEqual("Victoria", interchangeStop?.StopName);
    }


    /// <summary>
    ///     Test to identify the interchange between the Airport and East Didsbury.
    ///     This should be St Werburgh's Road.
    /// </summary>
    [Test]
    public void TestIdentifyInterchangeAirportDidsbury()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport")
            .ConvertToStopKeysV2();
        var didsburyStop = _importedStops?.First(stop => stop.StopName == "East Didsbury")
            .ConvertToStopKeysV2();
        var interchangeStop = _routeIdentifier?.IdentifyInterchangeStop(airportStop, didsburyStop);
        Assert.IsNotNull(interchangeStop);
        Assert.AreEqual("St Werburgh's Road", interchangeStop?.StopName);
    }

    /// <summary>
    ///     Test to identify the interchange with a null origin.
    ///     This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestIdentifyInterchangeNullOrigin()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport")
            .ConvertToStopKeysV2();

        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _routeIdentifier?.IdentifyInterchangeStop(null, airportStop);
            });
    }

    /// <summary>
    ///     Test to identify an interchange for a journey with a null destination.
    ///     This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestIdentifyInterchangeNullDestination()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport")
            .ConvertToStopKeysV2();
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _routeIdentifier?.IdentifyInterchangeStop(airportStop, null);
            });
    }

    /// <summary>
    ///     Test to Identify the routes between two stops.
    ///     This should contain 2 items, Purple and Green
    /// </summary>
    [Test]
    public void TestIdentifyRoutesBetweenStops()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham")
            .ConvertToStopKeysV2();
        var cornbrookStop = _importedStops?.First(stop => stop.StopName == "Cornbrook")
            .ConvertToStopKeysV2();

        var identifiedRoutes = _routeIdentifier?.IdentifyRoutesBetween(altrinchamStop, cornbrookStop);
        Assert.IsNotNull(identifiedRoutes);
        Assert.IsNotEmpty(identifiedRoutes ?? new List<RouteV2>());
        Assert.AreEqual(2, identifiedRoutes?.Count);
        Assert.That(identifiedRoutes!.Any(route => route.Name == "Purple"));
        Assert.That(identifiedRoutes!.Any(route => route.Name == "Green"));
    }


    /// <summary>
    ///     Test to identify the routes between Bury and Market Street.
    ///     This should identify the green and yellow route.
    /// </summary>
    [Test]
    public void TestIdentifyRoutesBetweenBuryMarketStreet()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury")
            .ConvertToStopKeysV2();
        var marketStreetStop = _importedStops?.First(stop => stop.StopName == "Market Street")
            .ConvertToStopKeysV2();
        var identifiedRoutes = _routeIdentifier?.IdentifyRoutesBetween(buryStop, marketStreetStop);
        Assert.IsNotNull(identifiedRoutes);
        Assert.IsNotEmpty(identifiedRoutes ?? new List<RouteV2>());
        Assert.AreEqual(2, identifiedRoutes?.Count);
        Assert.That(identifiedRoutes!.Any(route => route.Name == "Green"));
        Assert.That(identifiedRoutes!.Any(route => route.Name == "Yellow"));
    }


    /// <summary>
    ///     Identify routes with a null origin.
    ///     This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestIdentifyRoutesBetweenNullOrigin()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury")
            .ConvertToStopKeysV2();
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _routeIdentifier?.IdentifyRoutesBetween(null, buryStop);
            });
    }

    /// <summary>
    ///     Identify the routes between stops with a null destination.
    ///     This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestIdentifyRoutesBetweenNullDestination()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury")
            .ConvertToStopKeysV2();
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _routeIdentifier?.IdentifyRoutesBetween(buryStop, null);
            });
    }

    /// <summary>
    ///     Test to identify the terminus stop of a tram.
    ///     This should return the stop not where the route ends, but the
    ///     route terminates in the direction of travel.
    /// </summary>
    [Test]
    public void TestIdentifyTramTerminusForRoute()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford")
            .ConvertToStopKeysV2();
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands")
            .ConvertToStopKeysV2();
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        var identifiedTerminus = _routeIdentifier?
            .IdentifyRouteTerminus(stretfordStop, brooklandsStop, purpleRoute);
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham")
            .ConvertToStopKeysV2();
        Assert.IsNotNull(identifiedTerminus);
        Assert.AreEqual("Altrincham", identifiedTerminus?.StopName);
        Assert.AreEqual(altrinchamStop, identifiedTerminus);
    }

    /// <summary>
    ///     Test to identify the terminus of the purple route when going in the
    ///     opposite direction.
    ///     This should return Piccadilly.
    /// </summary>
    [Test]
    public void TestIdentifyTramTerminusForReverseRoute()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford")
            .ConvertToStopKeysV2();
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands")
            .ConvertToStopKeysV2();
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        var identifiedTerminus = _routeIdentifier?
            .IdentifyRouteTerminus(brooklandsStop, stretfordStop, purpleRoute);
        var piccadillyStop = _importedStops?.First(stop => stop.StopName == "Piccadilly")
            .ConvertToStopKeysV2();
        Assert.IsNotNull(identifiedTerminus);
        Assert.AreEqual("Piccadilly", identifiedTerminus?.StopName);
        Assert.AreEqual(piccadillyStop, identifiedTerminus);
    }


    /// <summary>
    ///     Test to identify a terminus with a null origin stop.
    ///     This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestIdentifyTerminusNullOrigin()
    {
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands")
            .ConvertToStopKeysV2();
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyRouteTerminus(null, brooklandsStop, purpleRoute);
            });
    }

    /// <summary>
    ///     Identify terminus with a null destination.
    ///     This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestIdentifyTerminusNullDestination()
    {
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands")
            .ConvertToStopKeysV2();
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyRouteTerminus(brooklandsStop, null, purpleRoute);
            });
    }

    /// <summary>
    ///     Test to identify a terminus with a null route.
    ///     This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestIdentifyTerminusNullRoute()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford")
            .ConvertToStopKeysV2();
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands")
            .ConvertToStopKeysV2();
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'route')"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyRouteTerminus(brooklandsStop, stretfordStop, null);
            });
    }

    /// <summary>
    ///     Test to identify a terminus of a route when the origin
    ///     stop does not belong to the route.
    ///     This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestIdentifyTerminusOriginNotOnRoute()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury")
            .ConvertToStopKeysV2();

        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands")
            .ConvertToStopKeysV2();

        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("Bury does not exist on Purple route"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyRouteTerminus(buryStop, brooklandsStop, purpleRoute);
            });
    }

    /// <summary>
    ///     Test to identify a terminus of a route when the destination
    ///     stop is not on the route
    ///     This should throw an invalid operation exception.
    /// </summary>
    [Test]
    public void TestIdentifyTerminusDestinationNotOnRoute()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury")
            .ConvertToStopKeysV2();
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands")
            .ConvertToStopKeysV2();
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("Bury does not exist on Purple route"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyRouteTerminus(brooklandsStop, buryStop, purpleRoute);
            });
    }
}