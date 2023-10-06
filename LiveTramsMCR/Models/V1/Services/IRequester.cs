using System.Collections.Generic;
using System.Net.Http;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Specifies the methods and outputs that must be implemented
///     for a class to act as a service requester.
/// </summary>
public interface IRequester
{
    /// <summary>
    ///     Requests the services for a given list of IDs.
    ///     These IDs should be retrieved from an associated tlaref.
    /// </summary>
    /// <param name="tlaref">Tlaref of the stop being requested</param>
    /// <returns></returns>
    public HttpResponseMessage RequestServices(string tlaref);

    /// <summary>
    /// Requests the live services for multiple tlarefs
    /// </summary>
    /// <param name="tlarefs">Tlarefs to get live services for</param>
    /// <returns>Response message of requests services</returns>
    public HttpResponseMessage RequestServices(IEnumerable<string> tlarefs);

    /// <summary>
    ///     Requests services for all IDs
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage RequestAllServices();
}