using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.Resources;

/// <summary>
/// Imports the routes from the routes file and attaches the Stops. 
/// </summary>
public class RouteLoader
{
    private ResourcesConfig _resourcesConfig;
    private List<Stop> _importedStops;
    
    /// <summary>
    /// Loads routes from the resources configuration and assigns the imported
    /// stops to those on a route.
    /// </summary>
    /// <param name="resourcesConfig">Configuration location of routes file</param>
    /// <param name="importedStops">Imported Stops to assign to Routes</param>
    public RouteLoader(ResourcesConfig resourcesConfig, List<Stop> importedStops)
    {
        _resourcesConfig = resourcesConfig;
        _importedStops = importedStops;
    }

    /// <summary>
    /// Imports routes from constructor resource configuration, and
    /// adds the relevant stop instances to the routes.
    /// </summary>
    /// <returns>List of Imported Routes</returns>
    public List<Route> ImportRoutes()
    {
        using var reader = new StreamReader(_resourcesConfig.RoutesResourcePath);
        var jsonString = reader.ReadToEnd();
        var unprocessedRoutes = JsonConvert.DeserializeObject<List<UnprocessedRoute>> (jsonString);
        var importedRoutes = new List<Route>();
        for(int i = 0; i < unprocessedRoutes?.Count; i++)
            importedRoutes.Add(new Route("", "", new List<Stop>()));
        return importedRoutes;
    }
}