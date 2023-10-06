using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Validates service information from
///     the HTTP responses from the service requester
/// </summary>
public static class ServiceValidator
{
    /// <summary>
    ///     Validates service response messages and returns each monitor as an unformatted service
    /// </summary>
    /// <param name="responseMessage">Http response message from service requester</param>
    /// <returns>List of unformatted services</returns>
    public static List<UnformattedServices> ValidateServiceResponse(HttpResponseMessage responseMessage)
    {
        var invalidResponseReturned = responseMessage is null ||
                                      responseMessage.StatusCode == HttpStatusCode.InternalServerError;
        if (invalidResponseReturned)
        {
            throw new InvalidOperationException("Retry in 5s");
        }

        responseMessage.EnsureSuccessStatusCode();
        var responseJson = responseMessage.Content.ReadAsStringAsync().Result;
        var deserializedServices = JsonConvert.DeserializeObject<MultipleUnformattedServices>(responseJson);

        return deserializedServices.Value;
    }
}