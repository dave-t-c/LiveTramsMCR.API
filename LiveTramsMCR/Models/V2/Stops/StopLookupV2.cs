using LiveTramsMCR.Models.V2.Stops.Data;

namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
/// Looks up a 
/// </summary>
public class StopLookupV2
{
    private readonly IStopsRepositoryV2 _stopsRepositoryV2;
    
    /// <summary>
    /// Creates a new stop lookup. The stops repository is used to lookup stops.
    /// </summary>
    /// <param name="stopsRepositoryV2"></param>
    public StopLookupV2(IStopsRepositoryV2 stopsRepositoryV2)
    {
        _stopsRepositoryV2 = stopsRepositoryV2;
    }

    /// <summary>
    /// Looks up a stop tlaref or name from a value string.
    /// </summary>
    public StopV2 LookupStop(string value)
    {
        var stop = _stopsRepositoryV2.GetStop(value);
        return stop;
    }
}