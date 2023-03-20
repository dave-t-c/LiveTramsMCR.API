using System.Threading.Tasks;

namespace LiveTramsMCR.Models.V1.RoutePlanner.Data;

/// <inheritdoc />
public class RouteRepository : IRouteRepository
{
    /// <inheritdoc />
    public Task<Route> GetRouteByNameAsync(string routeName)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public Task<RouteTimes> GetRouteTimesByNameAsync(string routeName)
    {
        throw new System.NotImplementedException();
    }
}