using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.Resources;

namespace LiveTramsMCR.Models.Stops;

/// <summary>
///     Looks up the Stop Information for a
///     given stop.
///     This will return the IDs from either a stop name, e.g. Victoria,
///     or a TLARef, e.g. ALT.
/// </summary>
public class StopLookup
{
    private readonly ImportedResources _importedResources;

    /// <summary>
    /// Generates a new StopLookup using the imported resources.
    /// Imported resources may be null here.
    /// </summary>
    /// <param name="importedResources">ImportedResources loaded at program start</param>
    public StopLookup(ImportedResources importedResources)
    {
        _importedResources = importedResources;
    }

    /// <summary>
    ///     Returns API IDs for a given Tlaref.
    /// </summary>
    /// <param name="tlaref">Stop Tlaref, e.g. 'ALT'.</param>
    /// <returns>Int list of IDs for the stop</returns>
    public List<int> TlarefLookup(string tlaref)
    {
        if (tlaref == null) throw new ArgumentNullException(nameof(tlaref));
        return _importedResources.TlarefsToIds[tlaref];
    }

    /// <summary>
    ///     Retrieves the IDs for a stop from a given stop name.
    /// </summary>
    /// <param name="stationName">Station name to retrieve, e.g. 'Ashton-Under-Lyne'</param>
    /// <returns>Int list of IDs to use with the Metrolink API.</returns>
    public List<int> StationNameLookup(string stationName)
    {
        if (stationName == null) throw new ArgumentNullException(nameof(stationName));
        var tlaref = _importedResources.StationNamesToTlaref[stationName];
        return TlarefLookup(tlaref);
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

        if (_importedResources.TlarefsToIds.ContainsKey(value)) return TlarefLookup(value);

        if (_importedResources.StationNamesToTlaref.ContainsKey(value)) return StationNameLookup(value);

        throw new ArgumentException("Value given is not a valid station name or TLAREF");
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
        return _importedResources.ImportedStops
            .First(stop => stop.StopName == value || stop.Tlaref == value);
    }
}