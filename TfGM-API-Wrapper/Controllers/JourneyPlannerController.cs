using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TfGM_API_Wrapper.Models.RoutePlanner;
using Swashbuckle.AspNetCore.Annotations;

namespace TfGM_API_Wrapper.Controllers;

/// <summary>
/// Controller for handling requests related to journey planning.
/// </summary>
[Route("/api/journey-planner")]
[ApiController]
public class JourneyPlannerController: Controller
{

    private readonly IJourneyPlannerModel _journeyPlannerModel;
    
    /// <summary>
    /// Creates a new JourneyPlannerController and uses the given IJourneyPlannerModel
    /// to process requests.
    /// </summary>
    /// <param name="journeyPlannerModel"></param>
    public JourneyPlannerController(IJourneyPlannerModel journeyPlannerModel)
    {
        _journeyPlannerModel = journeyPlannerModel;
    }

    /// <summary>
    /// Plans a journey between an origin and destination stop.
    /// </summary>
    /// <param name="origin">Origin stop name or tlaref</param>
    /// <param name="destination">Destination stop name or tlaref</param>
    /// <returns>A planned journey between the origin an destination</returns>
    [Route("/api/journey-planner/{origin}/{destination}")]
    [Produces("application/json")]
    [SwaggerResponse (type:typeof (PlannedJourney), statusCode: StatusCodes.Status200OK)]
    [HttpGet]
    public IActionResult PlanJourney(string origin, string destination)
    {
        PlannedJourney plannedJourney;
        try
        {
            plannedJourney = _journeyPlannerModel.PlanJourney(origin, destination);
        }
        catch (InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new {message = "Invalid Stop Name or TLAREF"});
        }
        return Ok(plannedJourney);
    }
}