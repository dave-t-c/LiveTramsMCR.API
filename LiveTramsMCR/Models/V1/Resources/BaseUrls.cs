namespace LiveTramsMCR.Models.V1.Resources;

/// <summary>
///     POCO for storing base urls for requests
/// </summary>
public class BaseUrls
{
    /// <summary>
    ///     Base URL for requesting live service information from the TfGM API.
    /// </summary>
    public string BaseLiveServicesUrl { get; set; }
}