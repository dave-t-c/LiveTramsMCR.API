using System;
using System.Collections.Generic;
using System.IO;
using LiveTramsMCR.Models.V2.RoutePlanner;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

public class RouteV2Loader
{
    private readonly ResourcesConfig? _resourcesConfig;

    public RouteV2Loader(ResourcesConfig resourcesConfig)
    {
        if (resourcesConfig.RoutesV2ResourcePath == null)
            return;
        
        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));
        _resourcesConfig.RoutesV2ResourcePath = LoaderHelper.CheckFileRequirements(
            resourcesConfig.RoutesV2ResourcePath,
            nameof(resourcesConfig.RoutesV2ResourcePath));
    }

    public List<RouteV2> ImportRoutes()
    {
        if (_resourcesConfig == null)
        {
            return new List<RouteV2>();
        }
        using var reader = new StreamReader(_resourcesConfig.RoutesV2ResourcePath!);
        var jsonString = reader.ReadToEnd();
        if (jsonString == null) throw new FileLoadException("Json could not be parsed to RouteV2");
        
        return JsonConvert.DeserializeObject<List<RouteV2>> (jsonString)!;
    }
}