using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper_Tests.TestModels.TestRoutePlanner;

/// <summary>
/// Test class for the RouteIdentifier.
/// </summary>
public class TestRouteIdentifier
{
    
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private ResourcesConfig? _validResourcesConfig;
    private StopLoader? _stopLoader;
    private List<Stop>? _importedStops;
    private RouteLoader? _routeLoader;
    private List<Route>? _routes;
    private RouteIdentifier? _routeIdentifier;
    
    /// <summary>
    /// Import routes / stops before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        //To make it easier to use different Stops in tests,
        //Stops are imported and then selected instead of created.
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesResourcePath
        };
        
        _stopLoader = new StopLoader(_validResourcesConfig);
        _importedStops = _stopLoader.ImportStops();

        _routeLoader = new RouteLoader(_validResourcesConfig, _importedStops);
        _routes = _routeLoader.ImportRoutes();

        _routeIdentifier = new RouteIdentifier(_routes);
    }

    /// <summary>
    /// Set stops and routes to null
    /// to ensure no cross-test contamination.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
        _importedStops = null;
        _stopLoader = null;
        _routes = null;
        _routeLoader = null;
        _routeIdentifier = null;
    }

    /// <summary>
    /// Test to determine if an interchange is required
    /// for two stops on the same route.
    /// This should return false.
    /// </summary>
    [Test]
    public void TestIsInterchangeRequired()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var saleStop = _importedStops?.First(stop => stop.StopName == "Sale");
        Assert.IsFalse(_routeIdentifier?.IsInterchangeRequired(altrinchamStop, saleStop));
    }

    /// <summary>
    /// Test to see if an interchange is required by checking stops on different routes.
    /// This should return true
    /// </summary>
    [Test]
    public void TestInterchangeShouldBeRequired()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var ashtonStop = _importedStops?.First(stop => stop.StopName == "Ashton Moss");
        Assert.IsTrue(_routeIdentifier?.IsInterchangeRequired(altrinchamStop, ashtonStop));
    }

    /// <summary>
    /// Test to determine if a null origin stop
    /// requires an interchange to a valid dest.
    /// This should throw an illegal args exception
    /// </summary>
    [Test]
    public void TestInterchangeNullOrigin()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _routeIdentifier?.IsInterchangeRequired(null, altrinchamStop);
            });
    }

    /// <summary>
    /// Test to determine if an interchange is required between a valid origin stop
    /// and a null dest.
    /// This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestInterchangeNullDest()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _routeIdentifier?.IsInterchangeRequired(altrinchamStop, null);
            });
    }

    /// <summary>
    /// Test to identify an interchange stop for a journey where an interchange is required.
    /// This should identify the stop shared on both routes that is closest to the destination.
    /// </summary>
    [Test]
    public void TestIdentifyInterChange()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var ashtonStop = _importedStops?.First(stop => stop.StopName == "Ashton Moss");
        var interchangeStop = _routeIdentifier?.IdentifyInterchangeStop(altrinchamStop, ashtonStop);
        Assert.NotNull(interchangeStop);
        Assert.AreEqual("Piccadilly", interchangeStop?.StopName);
    }

    
    /// <summary>
    /// Test to identify the interchange between Eccles and the Trafford Centre.
    /// This should return Pomona.
    /// </summary>
    [Test]
    public void TestIdentifyEcclesTraffordCentreInterchange()
    {
        var ecclesStop = _importedStops?.First(stop => stop.StopName == "Eccles");
        var traffordStop = _importedStops?.First(stop => stop.StopName == "The Trafford Centre");
        var interchangeStop = _routeIdentifier?.IdentifyInterchangeStop(ecclesStop, traffordStop);
        Assert.IsNotNull(interchangeStop);
        Assert.AreEqual("Pomona", interchangeStop?.StopName);
    }

    /// <summary>
    /// Test to identify the interchange between the Airport and Bury.
    /// This should return Victoria.
    /// </summary>
    [Test]
    public void TestIdentifyInterchangeAirportBury()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport");
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        var interchangeStop = _routeIdentifier?.IdentifyInterchangeStop(airportStop, buryStop);
        Assert.IsNotNull(interchangeStop);
        Assert.AreEqual("Victoria", interchangeStop?.StopName);
    }


    /// <summary>
    /// Test to identify the interchange between the Airport and East Didsbury.
    /// This should be St Werburgh's Road.
    /// </summary>
    [Test]
    public void TestIdentifyInterchangeAirportDidsbury()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport");
        var didsburyStop = _importedStops?.First(stop => stop.StopName == "East Didsbury");
        var interchangeStop = _routeIdentifier?.IdentifyInterchangeStop(airportStop, didsburyStop);
        Assert.IsNotNull(interchangeStop);
        Assert.AreEqual("St Werburgh's Road", interchangeStop?.StopName);
    }

    /// <summary>
    /// Test to identify the interchange with a null origin.
    /// This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestIdentifyInterchangeNullOrigin()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _routeIdentifier?.IdentifyInterchangeStop(null, airportStop);
            });
    }

    /// <summary>
    /// Test to identify an interchange for a journey with a null destination.
    /// This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestIdentifyInterchangeNullDestination()
    {
        var airportStop = _importedStops?.First(stop => stop.StopName == "Manchester Airport");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _routeIdentifier?.IdentifyInterchangeStop(airportStop, null);
            });
    }

    /// <summary>
    /// Test to Identify the routes between two stops.
    /// This should contain 2 items, Purple and Green
    /// </summary>
    [Test]
    public void TestIdentifyRoutesBetweenStops()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var cornbrookStop = _importedStops?.First(stop => stop.StopName == "Cornbrook");

        var identifiedRoutes = _routeIdentifier?.IdentifyRoutesBetween(altrinchamStop, cornbrookStop);
        Assert.IsNotNull(identifiedRoutes);
        Assert.IsNotEmpty(identifiedRoutes ?? new List<Route>());
        Assert.AreEqual(2, identifiedRoutes?.Count);
        Assert.That(identifiedRoutes!.Any(route => route.Name == "Purple"));
        Assert.That(identifiedRoutes!.Any(route => route.Name == "Green"));
    }


    /// <summary>
    /// Test to identify the routes between Bury and Market Street.
    /// This should identify the green and yellow route.
    /// </summary>
    [Test]
    public void TestIdentifyRoutesBetweenBuryMarketStreet()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        var marketStreetStop = _importedStops?.First(stop => stop.StopName == "Market Street");
        var identifiedRoutes = _routeIdentifier?.IdentifyRoutesBetween(buryStop, marketStreetStop);
        Assert.IsNotNull(identifiedRoutes);
        Assert.IsNotEmpty(identifiedRoutes ?? new List<Route>());
        Assert.AreEqual(2, identifiedRoutes?.Count);
        Assert.That(identifiedRoutes!.Any(route => route.Name == "Green"));
        Assert.That(identifiedRoutes!.Any(route => route.Name == "Yellow"));
    }


    /// <summary>
    /// Identify routes with a null origin.
    /// This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestIdentifyRoutesBetweenNullOrigin()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _routeIdentifier?.IdentifyRoutesBetween(null, buryStop);
            });
    }

    /// <summary>
    /// Identify the routes between stops with a null destination.
    /// This should throw a null args exception.
    /// </summary>
    [Test]
    public void TestIdentifyRoutesBetweenNullDestination()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _routeIdentifier?.IdentifyRoutesBetween(buryStop, null);
            });
    }

    /// <summary>
    /// Test to identify the stops between an
    /// origin and a destination on a given route.
    /// This should identify a single stop, and not include the origin or destination Stop.
    /// </summary>
    [Test]
    public void TestIdentifyIntermediateStops()
    {
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        var timperleyStop = _importedStops?.First(stop => stop.StopName == "Timperley");
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        var intermediateStops = _routeIdentifier?
            .IdentifyIntermediateStops(altrinchamStop, timperleyStop, purpleRoute);
        Assert.IsNotNull(intermediateStops);
        Assert.IsNotEmpty(intermediateStops!);
        Assert.AreEqual(1, intermediateStops?.Count);
        Assert.IsTrue(intermediateStops?.Any(stop => stop.StopName == "Navigation Road"));
    }

    /// <summary>
    /// Test to identify intermediate stops for a different journey.
    /// This should return the stops between Stretford and Brooklands
    /// starting with Dane Road.
    /// This should match the order they would be traversed in if travelled between.
    /// </summary>
    [Test]
    public void TestIdentifyIntermediateStopsDifferentRoute()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford");
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands");
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        var intermediateStops = _routeIdentifier?
            .IdentifyIntermediateStops(stretfordStop, brooklandsStop, purpleRoute);
        Assert.IsNotNull(intermediateStops);
        Assert.IsNotEmpty(intermediateStops!);
        Assert.AreEqual(2, intermediateStops?.Count);
        Assert.AreEqual(0, intermediateStops?.IndexOf(
            intermediateStops.First(stop => stop.StopName == "Dane Road"))
        );
        Assert.AreEqual(1, intermediateStops?.IndexOf(
            intermediateStops.First(stop => stop.StopName == "Sale"))
        );
    }

    /// <summary>
    /// Test to identify the same intermediate stops as the previous test,
    /// but as the origin and destination have been swapped, they should be in the
    /// opposite order.
    /// </summary>
    [Test]
    public void TestIdentifyIntermediateStopsReverse()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford");
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands");
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        var intermediateStops = _routeIdentifier?
            .IdentifyIntermediateStops(brooklandsStop, stretfordStop, purpleRoute);
        Assert.IsNotNull(intermediateStops);
        Assert.IsNotEmpty(intermediateStops!);
        Assert.AreEqual(2, intermediateStops?.Count);
        Assert.AreEqual(1, intermediateStops?.IndexOf(
            intermediateStops.First(stop => stop.StopName == "Dane Road"))
        );
        Assert.AreEqual(0, intermediateStops?.IndexOf(
            intermediateStops.First(stop => stop.StopName == "Sale"))
        );
    }

    /// <summary>
    /// Test to identify an intermediate stops with an origin Stop not on the route.
    /// This should throw an InvalidOperationException.
    /// </summary>
    [Test]
    public void TestIdentifyIntermediateStopsWithInvalidStop()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford");
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("Bury does not exist on Purple route"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyIntermediateStops(buryStop, stretfordStop, purpleRoute);
            });
    }

    /// <summary>
    /// Test to identify intermediate stops where the destination stop
    /// does not belong to the route.
    /// This should throw an invalid op exception.
    /// </summary>
    [Test]
    public void TestIdentifyIntermediateStopsWithInvalidDestinationStop()
    {
        var buryStop = _importedStops?.First(stop => stop.StopName == "Bury");
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford");
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("Bury does not exist on Purple route"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyIntermediateStops(stretfordStop, buryStop, purpleRoute);
            });
    }

    /// <summary>
    /// Test to identify intermediate stops with a null origin.
    /// This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestIdentifyIntermediateRouteNullOrigin()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford");
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'origin')"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyIntermediateStops(null, stretfordStop, purpleRoute);
            });
    }
    
    /// <summary>
    /// Test to identify intermediate stops with a null destination.
    /// This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestIdentifyIntermediateRouteNullDestination()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford");
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyIntermediateStops(stretfordStop, null, purpleRoute);
            });
    }
    
    /// <summary>
    /// Test to identify intermediate stops with a null route.
    /// This should throw an args null exception.
    /// </summary>
    [Test]
    public void TestIdentifyIntermediateRouteNullRoute()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford");
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands");
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'route')"),
            delegate
            {
                var unused = _routeIdentifier?
                    .IdentifyIntermediateStops(stretfordStop, brooklandsStop, null);
            });
    }

    /// <summary>
    /// Test to identify the terminus stop of a tram.
    /// This should return the stop not where the route ends, but the
    /// route terminates in the direction of travel.
    /// </summary>
    [Test]
    public void TestIdentifyTramTerminusForRoute()
    {
        var stretfordStop = _importedStops?.First(stop => stop.StopName == "Stretford");
        var brooklandsStop = _importedStops?.First(stop => stop.StopName == "Brooklands");
        var purpleRoute = _routes?.First(route => route.Name == "Purple");
        var identifiedTerminus = _routeIdentifier?
            .IdentifyRouteTerminus(stretfordStop, brooklandsStop, purpleRoute);
        var altrinchamStop = _importedStops?.First(stop => stop.StopName == "Altrincham");
        Assert.IsNotNull(identifiedTerminus);
        Assert.AreEqual("Altrincham", identifiedTerminus?.StopName);
    }
}