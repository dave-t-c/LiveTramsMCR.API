using System;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.Stops;

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
    /// <param name="resources">Imported resources for the application</param>
    public ServiceProcessor(IRequester requester, ImportedResources resources)
    {
        _requester = requester;
        _stopLookup = new StopLookup(resources);
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
        var unformattedServicesList = _requester.RequestServices(stopIds);
        return _serviceFormatter.FormatServices(unformattedServicesList);
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
        var unformattedServicesList = _requester.RequestServices(stopIds);
        return _serviceFormatter.FormatDepartureBoardServices(unformattedServicesList);
    }
}