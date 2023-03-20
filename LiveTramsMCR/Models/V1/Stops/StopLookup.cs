using System;
using System.Collections.Generic;
using LiveTramsMCR.Models.V1.Stops.Data;

namespace LiveTramsMCR.Models.V1.Stops;

/// <summary>
///     Looks up the Stop Information for a
///     given stop.
///     This will return the IDs from either a stop name, e.g. Victoria,
///     or a TLARef, e.g. ALT.
/// </summary>
public class StopLookup
{
    private readonly IStopsRepository _stopsRepository;

    /// <summary>
    /// Generates a new StopLookup using the imported resources.
    /// Imported resources may be null here.
    /// </summary>
    /// <param name="stopsRepository"></param>
    public StopLookup(IStopsRepository stopsRepository)
    {
        _stopsRepository = stopsRepository;
    }

    /// <summary>
    ///     Returns API IDs for a given Tlaref.
    /// </summary>
    /// <param name="tlaref">Stop Tlaref, e.g. 'ALT'.</param>
    /// <returns>Int list of IDs for the stop</returns>
    public List<int> TlarefLookup(string tlaref)
    {
        if (tlaref == null) throw new ArgumentNullException(nameof(tlaref));
        return _stopsRepository.GetStopIds(tlaref).Result;
    }

    /// <summary>
    ///     Retrieves the IDs for a stop from a given stop name.
    /// </summary>
    /// <param name="stationName">Station name to retrieve, e.g. 'Ashton-Under-Lyne'</param>
    /// <returns>Int list of IDs to use with the Metrolink API.</returns>
    public List<int> StationNameLookup(string stationName)
    {
        if (stationName == null) throw new ArgumentNullException(nameof(stationName));
        return _stopsRepository.GetStopIds(stationName).Result;
    }

    /// <summary>
    ///     Looks up either a tlaref or station name to find the Ids.
    ///     The function determines if it is a tlaref or station name.
    /// </summary>
    /// <param name="value">Needs to be either a Tlaref or Station name</param>
    /// <returns></returns>
    public List<int> LookupIDs(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var ids = _stopsRepository.GetStopIds(value).Result;
        if (ids == null)
        {
            throw new ArgumentException("Value given is not a valid station name or TLAREF");    
        }

        return ids;
    }

    /// <summary>
    /// Looks up a stop from either its stop name or TLAREF value.
    /// </summary>
    /// <param name="value">Stop name or TLAREF value for a stop.</param>
    /// <returns>Stop object with the associated name or TLAREF</returns>
    public Stop LookupStop(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        return _stopsRepository.GetStop(value).Result;
    }
}