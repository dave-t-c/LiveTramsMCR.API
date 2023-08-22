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
    private const string RoutesV2ResourcePath = "../../../Resources/RoutesV2.json";
    private const string StopsV2ResourcePath = "../../../Resources/StopsV2.json";
    private UnformattedServices? _unformattedServices;
    private List<StopV2>? _importedStops;
    private List<RouteV2>? _importedRoutes;
    private FormattedServices? _formattedServices;
    private NextServiceIdentifierV2? _nextServiceIdentifierV2;
    
    [SetUp]
    public void SetUp()
    {
        _unformattedServices = _unformattedServices = ImportServicesResponse.ImportUnformattedServices(LiveServicesResponsePath);
        var unformattedServicesList = new List<UnformattedServices>()
        {
            _unformattedServices!
        };
        _formattedServices = new ServiceFormatter().FormatServices(unformattedServicesList);
        
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
        var services = new SortedSet<Tram>(new TramComparer());
        foreach (var destination in _formattedServices!.Destinations)
        {
            var trams = destination.Value;
            var filteredTrams = trams.Where(tram => tram.Tlaref == originStop.Tlaref);
            services.UnionWith(filteredTrams);
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
        Assert.Fail();
    }

    /// <summary>
    /// Test to identify the next service when there is no match 
    /// </summary>
    [Test]
    public void TestIdentifyNextServiceNoMatch()
    {
        Assert.Fail();
    }
}