namespace LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;

/// <summary>
/// Identifies the next service from a location to a given stop
/// using the formatted services and routes provided
/// </summary>
public interface INextServiceIdentifierV2
{
    /// <summary>
    /// Identifies the next service from a given stop towards a destination / interchange
    /// using the route and service information given.
    /// This method will only identify a next service when the origin and destination are
    /// on the same route. 
    /// </summary>
    public NextServiceIdentifierV2Response IdentifyNextService(NextServiceIdentifierV2Request request);
}