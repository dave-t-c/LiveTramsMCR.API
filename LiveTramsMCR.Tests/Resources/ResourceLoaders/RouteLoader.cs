using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiveTramsMCR.Models.V1.RoutePlanner;
using LiveTramsMCR.Models.V1.Stops;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
/// Imports the routes from the routes file and attaches the Stops. 
/// </summary>
public class RouteLoader
{
    private readonly ResourcesConfig? _resourcesConfig;
    private readonly List<Stop>? _importedStops;
    private Dictionary<string, Stop> _stopsDictionary = new();

    /// <summary>
    /// Loads routes from the resources configuration and assigns the imported
    /// stops to those on a route.
    /// </summary>
    /// <param name="resourcesConfig">Configuration location of routes file</param>
    /// <param name="importedStops">Imported Stops to assign to Routes</param>
    public RouteLoader(ResourcesConfig resourcesConfig, List<Stop> importedStops)
    {
        var loaderHelper = new LoaderHelper();
        if (resourcesConfig.RoutesResourcePath == null)
            return;
        
        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));
        _importedStops = importedStops ?? throw new ArgumentNullException(nameof(importedStops));

        _resourcesConfig.RoutesResourcePath = LoaderHelper.CheckFileRequirements(resourcesConfig.RoutesResourcePath,
            nameof(resourcesConfig.RoutesResourcePath));
    }

    /// <summary>
    /// Imports routes from constructor resource configuration, and
    /// adds the relevant stop instances to the routes.
    /// </summary>
    /// <returns>List of Imported Routes</returns>
    public List<Route> ImportRoutes()
    {
        if (_resourcesConfig == null)
            return new List<Route>();
        
        using var reader = new StreamReader(_resourcesConfig.RoutesResourcePath!);
        var jsonString = reader.ReadToEnd();
        var unprocessedRoutes = JsonConvert.DeserializeObject<List<UnprocessedRoute>> (jsonString);
        var importedRoutes = new List<Route>();
        
        //Create a Stops Dictionary for faster lookup instead of having to through the list
        _stopsDictionary = new Dictionary<string, Stop>();
        foreach (var stop in _importedStops!)
        {
            _stopsDictionary[stop.StopName] = stop;
        }
        
        //Process all of the unprocessed routes, attaching the stops in the expected order
        Debug.Assert(unprocessedRoutes != null, nameof(unprocessedRoutes) + " != null");
        foreach (var unprocessedRoute in unprocessedRoutes)
        {
            var identifiedStops = unprocessedRoute.Stops
                .Select(stop => _stopsDictionary[stop]).ToList();
            importedRoutes.Add(new Route(unprocessedRoute.RouteName, unprocessedRoute.Colour, identifiedStops));
        }
        return importedRoutes;
    }
}