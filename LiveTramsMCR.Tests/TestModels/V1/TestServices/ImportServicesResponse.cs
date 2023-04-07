using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using LiveTramsMCR.Models.V1.Services;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

/// <summary>
///     Imports the services from the stored json test files into an UnformattedServices Object.
/// </summary>
public static class ImportServicesResponse
{
    /// <summary>
    ///     Reads a json file and returns an UnformattedServices object created using the api response.
    /// </summary>
    /// <param name="path">Path to services api response</param>
    /// <returns>Created UnformattedServices from given api response path.</returns>
    public static UnformattedServices? ImportUnformattedServices(string path)
    {
        using var reader = new StreamReader(path);
        var jsonString = reader.ReadToEnd();
        var deserializedServices = JsonConvert.DeserializeObject<MultipleUnformattedServices>(jsonString);
        return deserializedServices?.Value.First();
    }

    /// <summary>
    /// Reads a json file and returns the MultipleUnformattedServices obj for that file.
    /// Used for mocking endpoint response when requesting all services.
    /// </summary>
    /// <param name="path">Path to json file</param>
    /// <returns>Multiple unformatted services object created from json</returns>
    public static MultipleUnformattedServices? ImportMultipleUnformattedServices(string path)
    {
        using var reader = new StreamReader(path);
        var jsonString = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<MultipleUnformattedServices>(jsonString);
    }

    public static HttpResponseMessage? ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode statusCode, string contentPath)
    {
        using var reader = new StreamReader(contentPath);
        var jsonString = reader.ReadToEnd();
        var httpResponse = new HttpResponseMessage(statusCode);
        httpResponse.Content = new StringContent(jsonString);
        return httpResponse;
    }
}