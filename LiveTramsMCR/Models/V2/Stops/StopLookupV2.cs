using System;
using LiveTramsMCR.Models.V2.Stops.Data;

namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
///     Looks up the Stop Information for a
///     given stop.
///     This will return the IDs from either a stop name, e.g. Victoria,
///     or a TLARef, e.g. ALT.
/// </summary>
public class StopLookupV2
{
    private readonly IStopsRepositoryV2 _stopsRepository;

    /// <summary>
    /// Generates a new StopLookup using the imported resources.
    /// Imported resources may be null here.
    /// </summary>
    /// <param name="stopsRepository"></param>
    public StopLookupV2(IStopsRepositoryV2 stopsRepository)
    {
        _stopsRepository = stopsRepository;
    }
    
    /// <summary>
    /// Looks up a stop from either its stop name or TLAREF value.
    /// </summary>
    /// <param name="value">Stop name or TLAREF value for a stop.</param>
    /// <returns>Stop object with the associated name or TLAREF</returns>
    public StopV2 LookupStop(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        
        var stop = _stopsRepository.GetStop(value);
        if (stop == null)
        {
            throw new ArgumentException("Value given is not a valid station name or TLAREF");    
        }

        return stop;
    }
}