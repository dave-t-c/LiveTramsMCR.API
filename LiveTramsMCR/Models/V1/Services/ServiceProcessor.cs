using System;
using System.Collections.Generic;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Processes service requests
/// </summary>
public class ServiceProcessor : IServiceProcessor
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
    ///     Returns services for a given stop Tlaref.
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
    public FormattedServices RequestServices(PlannedJourneyV2 plannedJourneyV2)
    {
        if (plannedJourneyV2 == null) throw new ArgumentNullException(nameof(plannedJourneyV2));
        var stopTlarefs = new List<string>
        {
            plannedJourneyV2.OriginStop.Tlaref,
            plannedJourneyV2.DestinationStop.Tlaref
        };

        if (plannedJourneyV2.RequiresInterchange)
        {
            stopTlarefs.Add(plannedJourneyV2.InterchangeStop.Tlaref);
        }

        var servicesResponse = _requester.RequestServices(stopTlarefs);
        var unformattedServices = ServiceValidator.ValidateServiceResponse(servicesResponse);
        return _serviceFormatter.FormatServices(unformattedServices);
    }

    /// <summary>
    ///     Returns services formatted for a departure board
    ///     for a given stop Tlaref.
    /// </summary>
    /// <param name="stopTlaref">Stop tlaref</param>
    /// <returns>Service data formatted for use with a departure board</returns>
    public FormattedDepartureBoardServices RequestDepartureBoardServices(string stopTlaref)
    {
        if (stopTlaref == null) throw new ArgumentNullException(nameof(stopTlaref));
        var stop = _stopLookup.LookupStop(stopTlaref);
        var serviceResponses = _requester.RequestServices(stop.Tlaref);
        var unformattedServices = ServiceValidator.ValidateServiceResponse(serviceResponses);
        return _serviceFormatter.FormatDepartureBoardServices(unformattedServices);
    }
}