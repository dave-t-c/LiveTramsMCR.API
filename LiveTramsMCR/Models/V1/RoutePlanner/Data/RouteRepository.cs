using System.Threading.Tasks;

namespace LiveTramsMCR.Models.V1.RoutePlanner.Data;

public class RouteRepository : IRouteRepository
{
    public Task<Route> GetRouteByNameAsync(string routeName)
    {
        throw new System.NotImplementedException();
    }

    public Task<RouteTimes> GetRouteTimesByNameAsync(string routeName)
    {
        throw new System.NotImplementedException();
    }
}