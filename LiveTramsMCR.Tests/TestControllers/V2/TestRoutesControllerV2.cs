
using LiveTramsMCR.Models.V2.RoutePlanner.Data;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestControllers.V2;

public class TestRoutesControllerV2
{

    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private IRouteRepositoryV2? _routeRepositoryV2;

    [SetUp]
    public void Setup()
    {
        _resourcesConfig = new ResourcesConfig()
        {
            RoutesV2ResourcePath = "../../../Resources/RoutesV2.json"
        };
        _importedResources = new ResourceLoader(_resourcesConfig).ImportResources();
        _routeRepositoryV2 = new MockRouteRepositoryV2(_importedResources.ImportedRoutesV2);
    }

    [TearDown]
    public void TearDown()
    {
        _importedResources = null;
        _resourcesConfig = null;
    }

    /// <summary>
    /// Test to get the expected result code when getting all routes.
    /// This should return a 200 OK.
    /// </summary>
    [Test]
    public void TestGetAllRoutesExpectedResultCode()
    {
        
    }
}