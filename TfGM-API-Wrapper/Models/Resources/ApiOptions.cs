namespace TfGM_API_Wrapper.Models.Resources;

/// <summary>
/// Stores API Options used when requesting services
/// from the TfGM API.
/// These should be imported on project startup.
/// </summary>
public class ApiOptions
{
    /// <summary>
    /// TfGM API Subscription key
    /// </summary>
    public string OcpApimSubscriptionKey { get; set; }
}