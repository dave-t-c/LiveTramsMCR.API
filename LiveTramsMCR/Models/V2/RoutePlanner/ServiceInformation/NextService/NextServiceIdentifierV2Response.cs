using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;

/// <summary>
/// A destination, wait pair of the next service towards a destination.
/// </summary>
public class NextServiceIdentifierV2Response
{
    /// <summary>
    /// Destination of the next service towards the interchange / destination
    /// </summary>
    public StopKeysV2 Destination { get; set; }
    
    /// <summary>
    /// Wait time until this service
    /// </summary>
    public int Wait { get; set; }
}