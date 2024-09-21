using System;
using LiveTramsMCR.Models.V2.Stops.Data;

namespace LiveTramsMCR.Models.V2.Stops;

/// <inheritdoc />
public class StopLookupV2 : IStopLookupV2
{
    private readonly IStopsRepositoryV2 _stopsRepositoryV2;

    /// <summary>
    ///     Creates a new stop lookup. The stops repository is used to lookup stops.
    /// </summary>
    /// <param name="stopsRepositoryV2"></param>
    public StopLookupV2(IStopsRepositoryV2 stopsRepositoryV2)
    {
        _stopsRepositoryV2 = stopsRepositoryV2;
    }
    
    /// <inheritdoc />
    public StopV2 LookupStop(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var stop = _stopsRepositoryV2.GetStop(value);

        if (stop is null)
            throw new ArgumentException("Value given is not a valid station name or TLAREF");

        return stop;
    }
}