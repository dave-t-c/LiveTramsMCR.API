using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;
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
        using var routesReader = new StreamReader(_resourcesConfig.RoutesV2ResourcePath!);
        var jsonString = routesReader.ReadToEnd();
        if (jsonString == null) throw new FileLoadException("Json could not be parsed to RouteV2");

        var routes = JsonConvert.DeserializeObject<List<RouteV2>>(jsonString)!;

        using var stopsReader = new StreamReader(_resourcesConfig.StopV2ResourcePath!);
        var stopsJson = stopsReader.ReadToEnd();
        if (stopsJson == null) throw new FileLoadException("Json could not be parsed to StopsV2");
        var stops = JsonConvert.DeserializeObject<List<StopV2>>(stopsJson);

        foreach (var route in routes)
        {
            route.StopsDetail = new List<StopV2>();
            foreach (var stop in route.Stops)
            {
                route.StopsDetail.Add(stops!.Single(s => s.Tlaref == stop.Tlaref));
            }
        }


        return routes;
    }
}