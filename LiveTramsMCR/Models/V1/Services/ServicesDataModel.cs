using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops.Data;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
/// Data model that handles requests for service information.
/// </summary>
public class ServicesDataModel: IServicesDataModel
{
    
    private readonly ServiceProcessor _serviceProcessor;

    /// <summary>
    /// Creates a new Services model using provided resources and requester
    /// </summary>
    /// <param name="stopsRepository">Repository for retrieving stops</param>
    /// <param name="routeRepository">Repository for updating routes</param>
    /// <param name="requester">Requester responsible for live service requests.</param>
    public ServicesDataModel(IStopsRepository stopsRepository, IRouteRepository routeRepository, IRequester requester)
    {
        _serviceProcessor = new ServiceProcessor(requester, stopsRepository, routeRepository);
    }
    
    /// <summary>
    /// Requests services for a given stop name or tlaref.
    /// </summary>
    /// <param name="stop">Stop name or tlaref</param>
    /// <returns>Formatted Services at given stop</returns>
    public FormattedServices RequestServices(string stop)
    {
        return _serviceProcessor.RequestServices(stop);
    }

    /// <summary>
    /// Request services for a stop or tlaref formatted for a departure board
    /// </summary>
    /// <param name="stop">Stop name or tlaref</param>
    /// <returns>Services for the given stop</returns>
    public FormattedDepartureBoardServices RequestDepartureBoardServices(string stop)
    {
        return _serviceProcessor.RequestDepartureBoardServices(stop);
    }
}