using System;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Processes service requests
/// </summary>
public class ServiceProcessor
{
    private readonly IRequester _requester;
    private readonly ServiceFormatter _serviceFormatter;
    private readonly StopLookup _stopLookup;

    /// <summary>
    ///     Creates a new service processor using the imported resources and
    ///     IRequest implementation.
    /// </summary>
    /// <param name="requester">IRequester implementation to use</param>
    /// <param name="stopsRepository">Repository for retrieving stops</param>
    public ServiceProcessor(IRequester requester, IStopsRepository stopsRepository)
    {
        _requester = requester;
        _stopLookup = new StopLookup(stopsRepository);
        _serviceFormatter = new ServiceFormatter();
    }

    /// <summary>
    ///     Returns services for a given stop name or Tlaref.
    /// </summary>
    /// <param name="stopIdentifier">Stop Id, either name or Tlaref. </param>
    /// <returns>Formatted Services for given stop</returns>
    public FormattedServices RequestServices(string stopIdentifier)
    {
        if (stopIdentifier == null) throw new ArgumentNullException(nameof(stopIdentifier));
        var stop = _stopLookup.LookupStop(stopIdentifier);
        var serviceResponses = _requester.RequestServices(stop.Tlaref);
        var unformattedServices = ServiceValidator.ValidateServiceResponse(serviceResponses);
        return _serviceFormatter.FormatServices(unformattedServices);
    }

    /// <summary>
    /// Request the services for a planned journey.
    /// This will request the service information for origin and destination stops.
    /// The interchange stop will also be requested if the journey involves
    /// and interchange.
    /// </summary>
    public FormattedServices RequestServicesForPlannedJourneyV2(PlannedJourneyV2 plannedJourneyV2)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Returns services formatted for a departure board
    ///     for a given stop name or Tlaref.
    /// </summary>
    /// <param name="stopIdentifier">Stop identifier, either name or tlaref</param>
    /// <returns>Service data formatted for use with a departure board</returns>
    public FormattedDepartureBoardServices RequestDepartureBoardServices(string stopIdentifier)
    {
        if (stopIdentifier == null) throw new ArgumentNullException(nameof(stopIdentifier));
        var stop = _stopLookup.LookupStop(stopIdentifier);
        var serviceResponses = _requester.RequestServices(stop.Tlaref);
        var unformattedServices = ServiceValidator.ValidateServiceResponse(serviceResponses);
        return _serviceFormatter.FormatDepartureBoardServices(unformattedServices);
    }
}