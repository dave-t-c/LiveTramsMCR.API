using System.IO;
using LiveTramsMCR.Models.Services;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.TestModels.TestServices;

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
        return JsonConvert.DeserializeObject<UnformattedServices>(jsonString);
    }
}