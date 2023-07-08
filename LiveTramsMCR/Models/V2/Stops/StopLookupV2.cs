using LiveTramsMCR.Models.V2.Stops.Data;

namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
/// Looks up a 
/// </summary>
public class StopLookupV2
{
    public StopLookupV2()
    {
    }

    
    public StopV2 LookupStop(string value)
    {
        return new StopV2()
        {
            Tlaref = "ALT", StopName = "Altrincham"
        };
    }
}