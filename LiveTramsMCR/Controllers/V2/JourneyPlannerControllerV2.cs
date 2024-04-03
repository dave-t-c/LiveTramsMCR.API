using System;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiveTramsMCR.Controllers.V2;

/// <summary>
/// Controller for journey planner v2 request.
/// </summary>
[Route("/v2/journey-planner")]
[ApiController]
public class JourneyPlannerControllerV2 : Controller
{
    private readonly IJourneyPlannerModelV2 _journeyPlannerModelV2;
    
    /// <summary>
    /// Create a new journey planner v2 controller.
    /// </summary>
    public JourneyPlannerControllerV2(IJourneyPlannerModelV2 journeyPlannerModelV2)
    {
        _journeyPlannerModelV2 = journeyPlannerModelV2;
    }

    /// <summary>
    /// Plans a journey between an origin and destination stop.
    /// </summary>
    /// <param name="origin">Origin stop tlaref</param>
    /// <param name="destination">Destination stop tlaref</param>
    [HttpGet]
    [Tags("JourneyPlanner")]
    [Route("/v2/journey-planner/{origin}/{destination}")]
    [Produces("application/json")]
    [SwaggerOperation(OperationId = "v2-journey-planner")]
    [SwaggerResponse(type: typeof (JourneyPlannerV2ResponseBodyModel), statusCode: StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Stop TLAREF provided")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occured")]
    public IActionResult PlanJourney(string origin, string destination)
    {
        JourneyPlannerV2ResponseBodyModel plannedJourney;
        try
        {
            plannedJourney = _journeyPlannerModelV2.PlanJourney(origin, destination);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new
            {
                message = "Invalid Stop Name or TLAREF"
            });
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return Ok(plannedJourney);
    }
}