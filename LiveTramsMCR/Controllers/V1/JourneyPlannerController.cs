using System;
using LiveTramsMCR.Models.V1.RoutePlanner;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiveTramsMCR.Controllers.V1;

/// <summary>
///     Controller for handling requests related to journey planning.
/// </summary>
[Route("/v1/journey-planner")]
[ApiController]
public class JourneyPlannerController : Controller
{

    private readonly IJourneyPlannerModel _journeyPlannerModel;

    /// <summary>
    ///     Creates a new JourneyPlannerController and uses the given IJourneyPlannerModel
    ///     to process requests.
    /// </summary>
    /// <param name="journeyPlannerModel"></param>
    public JourneyPlannerController(IJourneyPlannerModel journeyPlannerModel)
    {
        _journeyPlannerModel = journeyPlannerModel;
    }

    /// <summary>
    ///     Plans a journey between an origin and destination stop.
    /// </summary>
    /// <param name="origin">Origin stop name or tlaref</param>
    /// <param name="destination">Destination stop name or tlaref</param>
    /// <returns>A planned journey between the origin an destination</returns>
    [Route("/v1/journey-planner/{origin}/{destination}")]
    [Tags("JourneyPlanner")]
    [Produces("application/json")]
    [SwaggerResponse(type: typeof (PlannedJourney), statusCode: StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Stop Name or TLAREF provided")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occured")]
    [HttpGet]
    public IActionResult PlanJourney(string origin, string destination)
    {
        PlannedJourney plannedJourney;
        try
        {
            plannedJourney = _journeyPlannerModel.PlanJourney(origin, destination);
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