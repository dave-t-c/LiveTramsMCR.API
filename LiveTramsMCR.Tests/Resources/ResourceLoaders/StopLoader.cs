using System;
using System.Collections.Generic;
using System.IO;
using LiveTramsMCR.Models.V1.Stops;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
///     Loads in the Stop data from the resources folder.
/// </summary>
public class StopLoader
{
    private readonly Tests.Resources.ResourceLoaders.ResourcesConfig _resourcesConfig;

    /// <summary>
    ///     Create a new StopLoader Object, which can import the required Stops.
    /// </summary>
    /// <param name="resourcesConfig"></param>
    public StopLoader(Tests.Resources.ResourceLoaders.ResourcesConfig resourcesConfig)
    {
        var loaderHelper = new Tests.Resources.ResourceLoaders.LoaderHelper();

        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));

        _resourcesConfig.StopResourcePath = loaderHelper.CheckFileRequirements(resourcesConfig.StopResourcePath,
            nameof(resourcesConfig.StopResourcePath));
    }

    /// <summary>
    ///     Imports the Stops from the Stops resources file.
    ///     The results from this are then returned with a GET to '/api/stops'.
    /// </summary>
    public List<Stop> ImportStops()
    {
        using var reader = new StreamReader(_resourcesConfig.StopResourcePath);
        var jsonString = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<List<Stop>>(jsonString);
    }
}