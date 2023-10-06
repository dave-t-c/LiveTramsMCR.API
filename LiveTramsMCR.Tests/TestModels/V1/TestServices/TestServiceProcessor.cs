using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

/// <summary>
///     Test class for the ServiceProcessor.
/// </summary>
public class TestServiceProcessor
{
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesPath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private const string ValidApiResponsePath = "../../../Resources/ExampleApiResponse.json";
    private const string InternalServerErrorResponsePath = "../../../Resources/ExampleApiInternalServerErrorResponse.json";
    private const string UpdateStopsStopResourcePathConst = "../../../Resources/TestStopUpdater/stops.json";
    private const string UpdateStopsRoutesResourcePath = "../../../Resources/TestStopUpdater/routes.json";
    private const string UpdateStopsRouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private const string UpdateStopsApiResponsePath = "../../../Resources/TestStopUpdater/ApiResponse.json";
    private const string StopsV2ResourcePath = "../../../Resources/StopsV2.json";
    private const string JourneyPlannerV2ResponsePath = "../../../Resources/JourneyPlannerV2ApiResponse.json";
    private const string JourneyPlannerV2WithInterchangeResponsePath = "../../../Resources/JourneyPlannerV2InterchangeApiResponse.json";
    private ImportedResources? _importedResources;
    private MockServiceRequester? _mockServiceRequester;
    private MockServiceRequester? _mockServiceRequesterInternalServerError;
    private MockServiceRequester? _mockServiceRequesterJourneyPlannerV2;
    private MockServiceRequester? _mockServiceRequesterJourneyPlannerV2Interchange;
    private MockStopsRepository? _mockStopsRepository;
    private MockStopsRepository? _mockStopsRepositoryUpdateStops;
    private ResourceLoader? _resourceLoader;
    private ServiceProcessor? _serviceProcessor;
    private ServiceProcessor? _serviceProcessorInternalServerError;
    private ServiceProcessor? _serviceProcessorJourneyPlannerV2;
    private ServiceProcessor? _serviceProcessorJourneyPlannerV2Interchange;
    private ImportedResources? _updateStopsImportedResources;
    private ResourcesConfig? _updateStopsResourceConfig;
    private ResourceLoader? _updateStopsResourceLoader;
    private ResourcesConfig? _validResourcesConfig;
    private PlannedJourneyV2? _plannedJourneyV2;
    private PlannedJourneyV2? _plannedJourneyV2WithInterchange;
    private List<StopV2> _importedStopV2S = new();

    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesPath,
            RouteTimesPath = RouteTimesPath,
            StopV2ResourcePath = StopsV2ResourcePath
        };

        _updateStopsResourceConfig = new ResourcesConfig
        {
            StopResourcePath = UpdateStopsStopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = UpdateStopsRoutesResourcePath,
            RouteTimesPath = UpdateStopsRouteTimesPath
        };

        _resourceLoader = new ResourceLoader(_validResourcesConfig);
        _updateStopsResourceLoader = new ResourceLoader(_updateStopsResourceConfig);
        _importedResources = _resourceLoader.ImportResources();
        _updateStopsImportedResources = _updateStopsResourceLoader.ImportResources();

        var mockHttpResponse =
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode.OK, ValidApiResponsePath);

        var mockHttpResponseInternalServerError =
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode.InternalServerError,
                InternalServerErrorResponsePath);

        var mockHttpResponseGetAllStops =
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode.OK,
                UpdateStopsApiResponsePath);

        _mockServiceRequester = new MockServiceRequester(mockHttpResponse!);

        _mockServiceRequesterInternalServerError = new MockServiceRequester(
            mockHttpResponseInternalServerError!,
            mockHttpResponseGetAllStops!);

        _mockStopsRepository = new MockStopsRepository(_importedResources.ImportedStops);
        _mockStopsRepositoryUpdateStops = new MockStopsRepository(_updateStopsImportedResources.ImportedStops);

        _serviceProcessor = new ServiceProcessor(_mockServiceRequester, _mockStopsRepository);

        _serviceProcessorInternalServerError = new ServiceProcessor(
            _mockServiceRequesterInternalServerError,
            _mockStopsRepositoryUpdateStops);
        
        _importedStopV2S = _importedResources.ImportedStopsV2;
        
        _plannedJourneyV2 = new PlannedJourneyV2()
        {
            OriginStop = _importedStopV2S.First(stop => stop.Tlaref == "ALT"),
            DestinationStop = _importedStopV2S.First(stop => stop.StopName == "Trafford Bar")
        };
        
        _plannedJourneyV2WithInterchange = new PlannedJourneyV2()
        {
            RequiresInterchange = true,
            OriginStop = _importedStopV2S.First(stop => stop.Tlaref == "ALT"),
            InterchangeStop = _importedStopV2S.First(stop => stop.StopName == "Trafford Bar"),
            DestinationStop = _importedStopV2S.First(stop => stop.StopName == "Burton Road")
        };
        
        var mockJourneyPlannerV2HttpResponse = 
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode.OK, JourneyPlannerV2ResponsePath);
        
        var mockJourneyPlannerV2InterchangeHttpResponse = 
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode.OK, JourneyPlannerV2WithInterchangeResponsePath);

        _mockServiceRequesterJourneyPlannerV2 = new MockServiceRequester(mockJourneyPlannerV2HttpResponse!);

        _mockServiceRequesterJourneyPlannerV2Interchange = new MockServiceRequester(mockJourneyPlannerV2InterchangeHttpResponse!);
        
        _serviceProcessorJourneyPlannerV2 = new ServiceProcessor(
            _mockServiceRequesterJourneyPlannerV2,
            _mockStopsRepository);

        _serviceProcessorJourneyPlannerV2Interchange = new ServiceProcessor(
            _mockServiceRequesterJourneyPlannerV2Interchange,
            _mockStopsRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _mockServiceRequester = null;
        _validResourcesConfig = null;
        _resourceLoader = null;
        _importedResources = null;
        _mockServiceRequester = null;
        _serviceProcessor = null;
        _serviceProcessorInternalServerError = null;
    }


    /// <summary>
    ///     This should take a stop name, and then return the expected formatted services.
    ///     This will use tge MockServiceRequester so we can test an expected value as the data
    ///     will be static.
    ///     This should return FormattedServices.
    /// </summary>
    [Test]
    public void TestProcessServicesStopName()
    {
        Debug.Assert(_serviceProcessor != null, nameof(_serviceProcessor) + " != null");
        var result = _serviceProcessor.RequestServices("BMR");
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Destinations.Count);
    }

    /// <summary>
    ///     Test to use the service processor with a null stop name.
    ///     This should throw a null argument exception
    /// </summary>
    [Test]
    public void TestServiceProcessorNullName()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stopIdentifier')"),
            delegate
            {
                Debug.Assert(_serviceProcessor != null, nameof(_serviceProcessor) + " != null");
                _serviceProcessor.RequestServices(null as string);
            });
    }

    /// <summary>
    ///     Test to request services in the format for a departure board.
    /// </summary>
    [Test]
    public void TestServicesDepartureBoard()
    {
        var result = _serviceProcessor?.RequestDepartureBoardServices("BMR");
        Assert.IsNotNull(result);
        var trams = result?.Trams;
        Assert.IsNotNull(trams);
        Assert.AreEqual(3, trams?.Count);
        var firstTram = trams?.First();
        Assert.AreEqual("0", firstTram?.Wait);
        Assert.AreEqual(3, trams?.Distinct().Count());
        Assert.AreEqual("23", trams?.Last().Wait);
    }


    /// <summary>
    ///     Test to try and request departure board services with a null stop name.
    ///     This should throw an arg null exception.
    /// </summary>
    [Test]
    public void TestServicesDepartureBoardNullStop()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stopIdentifier')"),
            delegate
            {
                _serviceProcessor?.RequestDepartureBoardServices(null);
            });
    }

    /// <summary>
    ///     Test to try and retrieve live services when the IDs are outdated.
    ///     This should update the IDs to that given in the mock response
    ///     This should throw an invalid operation exception
    /// </summary>
    [Test]
    public void TestServiceIDsOutdated()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
            .And.Message.EqualTo("Retry in 5s"), delegate
        {
            _serviceProcessorInternalServerError?.RequestServices("Example 1");
        });
    }
    
    /// <summary>
    /// Test to get services for a planned journey
    /// This journey does not have an interchange so should 
    /// </summary>
    [Test]
    public void TestServicesForPlannedJourneyV2()
    {
        var result = _serviceProcessorJourneyPlannerV2?.RequestServices(_plannedJourneyV2);
        Assert.IsNotNull(result);
        var destinations = result?.Destinations;
        Assert.IsNotNull(destinations);
        Assert.AreEqual(6, destinations?.Count);
        var originTrams = result?.Destinations.Values.SelectMany(set =>
            set.Where(tram => tram.SourceTlaref == _plannedJourneyV2?.OriginStop.Tlaref));
        var destinationTrams = result?.Destinations.Values.SelectMany(set =>
            set.Where(tram => tram.SourceTlaref == _plannedJourneyV2?.DestinationStop.Tlaref));
        
        Assert.IsNotNull(originTrams);
        Assert.IsNotNull(destinationTrams);
        Assert.AreEqual(3, originTrams?.Count());
        Assert.AreEqual(6, destinationTrams?.Count());
    }

    /// <summary>
    /// Get the services for a planned journey with an interchange.
    /// This should include information for the interchange stop.
    /// </summary>
    [Test]
    public void TestServicesForPlannedJourneyV2WithInterchange()
    {
        var result = _serviceProcessorJourneyPlannerV2Interchange?.RequestServices(_plannedJourneyV2WithInterchange);
        Assert.IsNotNull(result);
        var destinations = result?.Destinations;
        Assert.IsNotNull(destinations);
        Assert.AreEqual(7, destinations?.Count);
        var originTrams = result?.Destinations.Values.SelectMany(set =>
            set.Where(tram => tram.SourceTlaref == _plannedJourneyV2WithInterchange?.OriginStop.Tlaref));
        
        var destinationTrams = result?.Destinations.Values.SelectMany(set =>
            set.Where(tram => tram.SourceTlaref == _plannedJourneyV2WithInterchange?.DestinationStop.Tlaref));
        
        var interchangeTrams = result?.Destinations.Values.SelectMany(set =>
            set.Where(tram => tram.SourceTlaref == _plannedJourneyV2WithInterchange?.InterchangeStop.Tlaref));
        
        Assert.IsNotNull(originTrams);
        Assert.IsNotNull(destinationTrams);
        Assert.IsNotNull(interchangeTrams);
        Assert.AreEqual(3, originTrams?.Count());
        Assert.AreEqual(6, destinationTrams?.Count());
        Assert.AreEqual(6, interchangeTrams?.Count());
    }
    
    /// <summary>
    /// Test to get the services for a null journey.
    /// This should throw an arg null exception.
    /// </summary>
    [Test]
    public void TestServicesForPlannedJourneyV2NullJourney()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'plannedJourneyV2')"),
            delegate
            {
                _serviceProcessorJourneyPlannerV2?.RequestServices(null as PlannedJourneyV2);
            });
    }
}