using System.Collections.Generic;
using TfGM_API_Wrapper.Models.RoutePlanner;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.Resources;

/// <summary>
/// Imports the routes from the routes file and attaches the Stops. 
/// </summary>
public class RouteLoader
{
    public RouteLoader(ResourcesConfig resourcesConfig, List<Stop> importedStops)
    {
        
    }

    public List<Route> ImportRoutes()
    {
        List<Route> importedRoutes = new List<Route>();
        for (int i = 0; i < 8; i++)
        {
            importedRoutes.Add(new Route("", "", new List<Stop>()));
        }

        return importedRoutes;
    }
}