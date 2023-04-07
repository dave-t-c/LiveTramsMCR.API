using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LiveTramsMCR.Models.V1.Resources;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
/// Handles requests for services from the TfGM API.
/// </summary>
public class ServiceRequester : IRequester
{
    private readonly ApiOptions _apiOptions;

    /// <summary>
    /// Creates a new ServiceRequester using an injected IConfiguration.
    /// config has a default of null in case it is not injected or has not been configured.
    /// </summary>
    /// <param name="apiOptions">API Options imported on startup. These are injected</param>
    public ServiceRequester(ApiOptions apiOptions)
    {
        _apiOptions = apiOptions;
    }

    /// <inheritdoc />
    public HttpResponseMessage RequestServices(string tlaref)
    {
        return RequestStopTlaref(tlaref).Result;
    }

    /// <summary>
    /// Requests the service information from a given ID using the TfGM API.
    /// </summary>
    /// <param name="tlaref">tlaref of stop being searched for</param>
    /// <returns>Unformatted Service for the given ID</returns>
    private async Task<HttpResponseMessage> RequestStopTlaref(string tlaref)
    {
        var client = new HttpClient();
        
        // Request headers
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiOptions.OcpApimSubscriptionKey);

        const string url = "https://api.tfgm.com/odata/Metrolinks";
        var filter = $"?$filter=TLAREF eq '{tlaref}'";

        var generatedUrl = url + filter;
        
        var response = await client.GetAsync(generatedUrl);
        return response;
    }
    
    /// <inheritdoc />
    public HttpResponseMessage RequestAllServices()
    {
        var client = new HttpClient();
        
        // Request headers
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiOptions.OcpApimSubscriptionKey);
        const string uri = "https://api.tfgm.com/odata/Metrolinks";

        var response = client.GetAsync(uri).Result;
        return response;
    }
}