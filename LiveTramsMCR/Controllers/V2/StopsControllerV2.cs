using System.Collections.Generic;
using LiveTramsMCR.Models.V2.Stops;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiveTramsMCR.Controllers.V2;

/// <summary>
/// Controller for Stops related information
/// </summary>
[Route("/v2/stops")]
[ApiController]
public class StopsControllerV2 : Controller
{
    private readonly IStopsDataModelV2 _stopsDataModel;

    /// <summary>
    ///     Controller Constructor that allows for custom stops path file location.
    ///     This can be used for testing the controller.
    ///     Default = Default location for stop data file. This should ideally be
    ///     extracted to the properties file.
    /// </summary>
    /// <param name="stopsDataModel">Data Model used for processing stops information</param>
    public StopsControllerV2(IStopsDataModelV2 stopsDataModel)
    {
        _stopsDataModel = stopsDataModel;
    }

    /// <summary>
    ///     Returns a JSON List of all Stops.
    /// </summary>
    /// <returns>JSON List -> Stop</returns>
    [Route("/v2/stops")]
    [Produces("application/json")]
    [SwaggerResponse (type:typeof (List<StopV2>), statusCode: StatusCodes.Status200OK)]
    [Tags("Stops")]
    [HttpGet]
    public IActionResult GetAllStops()
    {
        return Ok(_stopsDataModel.GetStops());
    }
}