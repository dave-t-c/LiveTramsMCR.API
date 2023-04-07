using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using Newtonsoft.Json;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
/// Validates service information from
/// the HTTP responses from the service requester
/// </summary>
public class ServiceValidator
{

    private readonly IStopsRepository _stopsRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IRequester _requester;
    
    /// <summary>
    /// Creates a new stop validator using the stops and route repository
    /// to update the IDs if required
    /// </summary>
    public ServiceValidator(IStopsRepository stopsRepository, IRouteRepository routeRepository, IRequester requester)
    {
        _stopsRepository = stopsRepository;
        _routeRepository = routeRepository;
        _requester = requester;
    }
    
    /// <summary>
    /// Validates service response messages and returns each monitor as an unformatted service
    /// </summary>
    /// <param name="responseMessages">Http response messages from service requester</param>
    /// <returns>List of unformatted services</returns>
    public List<UnformattedServices> ValidateServiceResponse(List<HttpResponseMessage> responseMessages)
    {
        var unformattedServices = new List<UnformattedServices>();
        var invalidResponseReturned = !responseMessages.Any() || 
                               responseMessages.Any(msg => msg.StatusCode == HttpStatusCode.InternalServerError);
        if (invalidResponseReturned)
        {
            throw new InvalidOperationException("Retry in 5s");
        }
        foreach (var httpResponse in responseMessages)
        {
            httpResponse.EnsureSuccessStatusCode();
            var responseJson = httpResponse.Content.ReadAsStringAsync().Result;
            unformattedServices.Add(JsonConvert.DeserializeObject<UnformattedServices>(responseJson));
        }
        return unformattedServices;
    }
}