using System.Collections.Generic;
using LiveTramsMCR.Models.V1.Stops;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiveTramsMCR.Controllers.V1;

/// <summary>
/// Controller for Stops related information
/// </summary>
[Route("/v1/stops")]
[ApiController]
public class StopsController : Controller
{
    private readonly IStopsDataModel _stopsDataModel;

    /// <summary>
    ///     Controller Constructor that allows for custom stops path file location.
    ///     This can be used for testing the controller.
    ///     Default = Default location for stop data file. This should ideally be
    ///     extracted to the properties file.
    /// </summary>
    /// <param name="stopsDataModel">Data Model used for processing stops information</param>
    public StopsController(IStopsDataModel stopsDataModel)
    {
        _stopsDataModel = stopsDataModel;
    }

    /// <summary>
    ///     Returns a JSON List of all Stops.
    /// </summary>
    /// <returns>JSON List -> Stop</returns>
    [Route("/v1/stops")]
    [Produces("application/json")]
    [SwaggerResponse (type:typeof (List<Stop>), statusCode: StatusCodes.Status200OK)]
    [HttpGet]
    public IActionResult GetAllStops()
    {
        return Ok(_stopsDataModel.GetStops());
    }
}