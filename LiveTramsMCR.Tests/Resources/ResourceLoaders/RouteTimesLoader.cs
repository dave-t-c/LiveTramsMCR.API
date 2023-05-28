using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiveTramsMCR.Models.V1.RoutePlanner;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
/// Loads route times information
/// </summary>
public class RouteTimesLoader
{

    private readonly ResourcesConfig? _resourcesConfig;
    
    /// <summary>
    /// Creates a new route times loader using the resources config.
    /// The resources config is used for the route times file location.
    /// </summary>
    /// <param name="resourcesConfig"></param>
    public RouteTimesLoader(ResourcesConfig resourcesConfig)
    {
        var loaderHelper = new LoaderHelper();
        if (resourcesConfig.RouteTimesPath == null)
            return;
        
        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));
        
        _resourcesConfig.RouteTimesPath = LoaderHelper.CheckFileRequirements(resourcesConfig.RouteTimesPath,
            nameof(resourcesConfig.RouteTimesPath));
    }

    /// <summary>
    /// Imports route times using the resources config given to the constructor
    /// </summary>
    /// <returns>Imported Times for Routes</returns>
    public List<RouteTimes> ImportRouteTimes()
    {
        if (_resourcesConfig == null)
            return new List<RouteTimes>();
        using var reader = new StreamReader(_resourcesConfig.RouteTimesPath!);
        var jsonString = reader.ReadToEnd();
        var unprocessedRouteTimes = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>> (jsonString);
        var createdRouteTimes = new List<RouteTimes>();
        foreach (var routeName in unprocessedRouteTimes!.Keys)
        {
            createdRouteTimes.Add((new RouteTimes
            {
                Route = routeName,
                Times = ProcessesRoute(unprocessedRouteTimes[routeName])
            }));
        }
        return createdRouteTimes;
    }

    /// <summary>
    /// Converts the Stop name -> Time stamp string to
    /// a StopName -> Timestamp
    /// </summary>
    /// <param name="unprocessedRoute">Unprocessed route times to convert to timestamps</param>
    /// <returns>A stop name, TimeStamp dict</returns>
    private static Dictionary<string, TimeSpan> ProcessesRoute(Dictionary<string, string> unprocessedRoute)
    {
        return unprocessedRoute.ToDictionary(
            kvp => kvp.Key, 
            kvp => TimeSpan.Parse(kvp.Value));
    }
}