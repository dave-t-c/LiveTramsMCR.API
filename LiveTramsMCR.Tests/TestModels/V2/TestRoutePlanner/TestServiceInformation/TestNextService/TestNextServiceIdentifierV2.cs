using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using LiveTramsMCR.Tests.TestModels.V1.TestServices;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V2.TestRoutePlanner.TestServiceInformation.TestNextService;

/// <summary>
/// Test class for the NextServiceIdentifierV2 class
/// </summary>
public class TestNextServiceIdentifierV2
{
    private const string LiveServicesResponsePath = "../../../Resources/ExampleAltrinchamResponse.json";
    private const string LiveCornbrookServicesResponsePath = "../../../Resources/ExampleCornbrookResponse.json";
    private const string LiveServicesNoServicesResponsePath = "../../../Resources/ExampleAltrinchamResponseNoServiceData.json";
    private const string RoutesV2ResourcePath = "../../../Resources/RoutesV2.json";
    private const string StopsV2ResourcePath = "../../../Resources/StopsV2.json";
    private MultipleUnformattedServices? _unformattedAltrinchamServices;
    private MultipleUnformattedServices? _unformattedCornbrookServices;
    private MultipleUnformattedServices? _unformattedServicesWithNoServiceData;
    private List<StopV2>? _importedStops;
    private List<RouteV2>? _importedRoutes;
    private FormattedServices? _formattedAltrinchamServices;
    private FormattedServices? _formattedCornbrookServices;
    private FormattedServices? _formattedServicesNoServiceData;
    private NextServiceIdentifierV2? _nextServiceIdentifierV2;
    
    [SetUp]
    public void SetUp()
    {
        var serviceFormatter = new ServiceFormatter();
        _unformattedAltrinchamServices = _unformattedAltrinchamServices = ImportServicesResponse.ImportMultipleUnformattedServices(LiveServicesResponsePath);
        _formattedAltrinchamServices = serviceFormatter.FormatServices(_unformattedAltrinchamServices!.Value);
        
        _unformattedCornbrookServices = ImportServicesResponse.ImportMultipleUnformattedServices(LiveCornbrookServicesResponsePath);
        _formattedCornbrookServices = serviceFormatter.FormatServices(_unformattedCornbrookServices!.Value);

        _unformattedServicesWithNoServiceData =
            ImportServicesResponse.ImportMultipleUnformattedServices(LiveServicesNoServicesResponsePath);
        _formattedServicesNoServiceData = serviceFormatter.FormatServices(_unformattedServicesWithNoServiceData!.Value);
        
        var resourcesConfig = new ResourcesConfig
        {
            RoutesV2ResourcePath = RoutesV2ResourcePath,
            StopV2ResourcePath = StopsV2ResourcePath
        };

        var stopsV2Loader = new StopV2Loader(resourcesConfig);
        var routesV2Loader = new RouteV2Loader(resourcesConfig);

        _importedStops = stopsV2Loader.ImportStops();
        _importedRoutes = routesV2Loader.ImportRoutes();
        _nextServiceIdentifierV2 = new NextServiceIdentifierV2();
    }

    [TearDown]
    public void TearDown()
    {
        
    }

    /// <summary>
    /// Test to identify the next service from ALT to Cornbrook.
    /// This should return Piccadilly in x mins.
    /// </summary>
    [Test]
    public void TestIdentifyNextService()
    {
        var originStop = _importedStops!.Single(stop => stop.Tlaref == "ALT");
        var destinationStop = _importedStops!.Single(stop => stop.StopName == "Cornbrook");
        var routeNames = new List<string>()
        {
            "Purple", "Green"
        };
        var routesFromOrigin = _importedRoutes!.Where(route => routeNames.Contains(route.Name)).ToList();
        var services = new List<Tram>();
        foreach (var destination in _formattedAltrinchamServices!.Destinations)
        {
            var trams = destination.Value;
            var filteredTrams = trams.Where(tram => tram.Tlaref == originStop.Tlaref);
            services.AddRange(filteredTrams);
        }
        
        var request = new NextServiceIdentifierV2Request()
        {
            Origin = originStop, Destination = destinationStop, Routes = routesFromOrigin, Services = services
        };

        var response = _nextServiceIdentifierV2!.IdentifyNextService(request);
        
        Assert.IsNotNull(response);
        var piccadillyStop = _importedStops!.Single(stop => stop.Tlaref == "PIC");
        var piccadillyStopKeys = new StopKeysV2()
        {
            StopName = piccadillyStop.StopName, Tlaref = piccadillyStop.Tlaref
        };
        Assert.AreEqual(piccadillyStopKeys, response.Destination);
        Assert.AreEqual(1, response.Wait);

    }

    /// <summary>
    /// Test to identify the next service in the opposite direction.
    /// This will need to do it in the opposite direction.
    /// </summary>
    [Test]
    public void TestIdentifyNextServiceOppositeDirection()
    {
        var destinationStop = _importedStops!.Single(stop => stop.Tlaref == "ALT");
        var originStop = _importedStops!.Single(stop => stop.StopName == "Cornbrook");
        var routeNames = new List<string>()
        {
            "Purple", "Green"
        };
        var routesFromOrigin = _importedRoutes!.Where(route => routeNames.Contains(route.Name)).ToList();
        var services = new List<Tram>();
        foreach (var destination in _formattedCornbrookServices!.Destinations)
        {
            var trams = destination.Value;
            var filteredTrams = trams.Where(tram => tram.Tlaref == originStop.Tlaref);
            services.AddRange(filteredTrams);
        }
        
        var request = new NextServiceIdentifierV2Request()
        {
            Origin = originStop, Destination = destinationStop, Routes = routesFromOrigin, Services = services
        };

        var response = _nextServiceIdentifierV2!.IdentifyNextService(request);
        
        Assert.IsNotNull(response);
        var altrinchamStopKeys = new StopKeysV2()
        {
            StopName = destinationStop.StopName, Tlaref = destinationStop.Tlaref
        };
        Assert.AreEqual(altrinchamStopKeys, response.Destination);
        Assert.AreEqual(3, response.Wait);
    }

    /// <summary>
    /// Test to identify the next service when there is no match
    /// This should return null
    /// </summary>
    [Test]
    public void TestIdentifyNextServiceNoMatch()
    {
        var originStop = _importedStops!.Single(stop => stop.Tlaref == "ALT");
        var destinationStop = _importedStops!.Single(stop => stop.StopName == "Cornbrook");
        var routeNames = new List<string>()
        {
            "Purple", "Green"
        };
        var routesFromOrigin = _importedRoutes!.Where(route => routeNames.Contains(route.Name)).ToList();
        var services = new List<Tram>();
        foreach (var destination in _formattedServicesNoServiceData!.Destinations)
        {
            var trams = destination.Value;
            var filteredTrams = trams.Where(tram => tram.Tlaref == originStop.Tlaref);
            services.AddRange(filteredTrams);
        }
        
        var request = new NextServiceIdentifierV2Request()
        {
            Origin = originStop, Destination = destinationStop, Routes = routesFromOrigin, Services = services
        };

        var response = _nextServiceIdentifierV2!.IdentifyNextService(request);
        
        Assert.IsNull(response);
    }
}