using System.Collections.Generic;
using LiveTramsMCR.Models.V2.RoutePlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiveTramsMCR.Controllers.V2;

/// <summary>
/// Controller for retrieving route data
/// </summary>
[Route("/v2/routes")]
[ApiController]
public class RoutesControllerV2 : Controller
{
    private readonly IRoutesDataModelV2 _routesDataModelV2;

    /// <summary>
    /// Creates a new routes controller using the route data model given.
    /// The route data model will be used to query data.
    /// </summary>
    /// <param name="routesDataModelV2"></param>
    public RoutesControllerV2(IRoutesDataModelV2 routesDataModelV2)
    {
        _routesDataModelV2 = routesDataModelV2;
    }

    /// <summary>
    /// Returns a json list of all routes.
    /// </summary>
    /// <returns></returns>
    [Route("/v2/routes")]
    [Produces("application/json")]
    [SwaggerResponse (type:typeof (List<RouteV2>), statusCode: StatusCodes.Status200OK)]
    [Tags("Routes")]
    [HttpGet]
    public IActionResult GetRoutes()
    {
        return Ok(_routesDataModelV2.GetAllRoutes());
    }
}