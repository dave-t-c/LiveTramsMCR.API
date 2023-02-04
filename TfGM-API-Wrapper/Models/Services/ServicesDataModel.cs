using TfGM_API_Wrapper.Models.Resources;

namespace TfGM_API_Wrapper.Models.Services;

/// <summary>
/// Data model that handles requests for service information.
/// </summary>
public class ServicesDataModel: IServicesDataModel
{
    
    private readonly ServiceProcessor _serviceProcessor;

    /// <summary>
    /// Creates a new Services model using provided resources and requester
    /// </summary>
    /// <param name="importedResources">Imported resources to be used for stops infromation</param>
    /// <param name="requester">Requester responsible for live service requests.</param>
    public ServicesDataModel(ImportedResources importedResources, IRequester requester)
    {
        _serviceProcessor = new ServiceProcessor(requester, importedResources);
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