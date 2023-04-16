namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
/// Stores keys required to uniquely identify a stop
/// </summary>
public class StopKeysV2
{
    /// <summary>
    /// Name of the stop, such as Piccadilly
    /// </summary>
    public string StopName { get; set; }
    
    /// <summary>
    /// 3 code ID for the stop, e.g. PIC for Piccadilly 
    /// </summary>
    public string Tlaref { get; set; }
}