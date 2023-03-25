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
    /// Requests the services for a given list of IDs.
    /// These IDs should be retrieved from an associated tlaref.
    /// </summary>
    /// <param name="ids">IDs to request services for</param>
    /// <returns></returns>
    public List<HttpResponseMessage> RequestServices(List<int> ids);

    /// <summary>
    /// Requests services for all IDs
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage RequestAllServices();
}