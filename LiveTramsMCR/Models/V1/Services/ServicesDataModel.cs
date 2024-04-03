using LiveTramsMCR.Models.V1.Stops.Data;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Data model that handles requests for service information.
/// </summary>
public class ServicesDataModel : IServicesDataModel
{

    private readonly ServiceProcessor _serviceProcessor;

    /// <summary>
    ///     Creates a new Services model using provided resources and requester
    /// </summary>
    /// <param name="stopsRepository">Repository for retrieving stops</param>
    /// <param name="requester">Requester responsible for live service requests.</param>
    public ServicesDataModel(IStopsRepository stopsRepository, IRequester requester)
    {
        _serviceProcessor = new ServiceProcessor(requester, stopsRepository);
    }

    /// <summary>
    ///     Requests services for a given stop tlaref.
    /// </summary>
    /// <param name="stop">Stop tlaref</param>
    /// <returns>Formatted Services at given stop</returns>
    public FormattedServices RequestServices(string stop)
    {
        return _serviceProcessor.RequestServices(stop);
    }

    /// <summary>
    ///     Request services for a tlaref formatted for a departure board
    /// </summary>
    /// <param name="stop">Stop or tlaref</param>
    /// <returns>Services for the given stop</returns>
    public FormattedDepartureBoardServices RequestDepartureBoardServices(string stop)
    {
        return _serviceProcessor.RequestDepartureBoardServices(stop);
    }
}