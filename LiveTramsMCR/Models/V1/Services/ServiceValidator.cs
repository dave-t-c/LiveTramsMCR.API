using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
/// Validates service information from
/// the HTTP responses from the service requester
/// </summary>
public class ServiceValidator
{
    /// <summary>
    /// Validates service response messages and returns each monitor as an unformatted service
    /// </summary>
    /// <param name="responseMessages">Http response messages from service requester</param>
    /// <returns>List of unformatted services</returns>
    public List<UnformattedServices> ValidateServiceResponse(List<HttpResponseMessage> responseMessages)
    {
        var unformattedServices = new List<UnformattedServices>();
        foreach (var httpResponse in responseMessages)
        {
            var responseJson = httpResponse.Content.ReadAsStringAsync().Result;
            unformattedServices.Add(JsonConvert.DeserializeObject<UnformattedServices>(responseJson));
        }
        return unformattedServices;
    }
}