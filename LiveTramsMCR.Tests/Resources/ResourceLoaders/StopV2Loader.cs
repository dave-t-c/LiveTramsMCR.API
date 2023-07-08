using System;
using System.Collections.Generic;
using System.IO;
using LiveTramsMCR.Models.V2.Stops;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
///     Loads in the Stop data from the resources folder.
/// </summary>
public class StopV2Loader
{
    private readonly ResourcesConfig _resourcesConfig;

    /// <summary>
    ///     Create a new StopLoader Object, which can import the required Stops.
    /// </summary>
    /// <param name="resourcesConfig"></param>
    public StopV2Loader(ResourcesConfig resourcesConfig)
    {
        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));
    }

    /// <summary>
    ///     Imports the Stops from the Stops resources file.
    ///     The results from this are then returned with a GET to '/api/v2/stops'.
    /// </summary>
    public List<StopV2> ImportStops()
    {
        if (_resourcesConfig.StopV2ResourcePath == null)
        {
            return new List<StopV2>();
        }
        using var reader = new StreamReader(_resourcesConfig.StopV2ResourcePath!);
        var jsonString = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<List<StopV2>>(jsonString)!;
    }
}