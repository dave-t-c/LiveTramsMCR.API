using System;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
/// Processes service requests
/// </summary>
public class ServiceProcessor
{
    private readonly IRequester _requester;
    private readonly ServiceFormatter _serviceFormatter;
    private readonly StopLookup _stopLookup;

    /// <summary>
    /// Creates a new service processor using the imported resources and
    /// IRequest implementation.
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
    /// <param name="stop">Stop Id, either name or Tlaref. </param>
    /// <returns>Formatted Services for given stop</returns>
    public FormattedServices RequestServices(string stop)
    {
        if (stop == null) throw new ArgumentNullException(nameof(stop));
        var stopIds = _stopLookup.LookupIDs(stop);
        var serviceResponses = _requester.RequestServices(stopIds);
        var unformattedServices = ServiceValidator.ValidateServiceResponse(serviceResponses);
        return _serviceFormatter.FormatServices(unformattedServices);
    }

    /// <summary>
    /// Returns services formatted for a departure board
    /// for a given stop name or Tlaref.
    /// </summary>
    /// <param name="stop">Stop identifier, either name or tlaref</param>
    /// <returns>Service data formatted for use with a departure board</returns>
    public FormattedDepartureBoardServices RequestDepartureBoardServices(string stop)
    {
        if (stop == null) throw new ArgumentNullException(nameof(stop));
        var stopIds = _stopLookup.LookupIDs(stop);
        var serviceResponses = _requester.RequestServices(stopIds);
        var unformattedServices = ServiceValidator.ValidateServiceResponse(serviceResponses);
        return _serviceFormatter.FormatDepartureBoardServices(unformattedServices);
    }
}