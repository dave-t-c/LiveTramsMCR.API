using System.Collections.Generic;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
/// IComparer used when determining which tram should be included
/// in results first.
/// </summary>
public class TramComparer : IComparer<Tram>
{
    /// <summary>
    /// CompareTo equivalent for two trams
    /// </summary>
    /// <param name="aTram">Tram to compare (can be null)</param>
    /// <param name="bTram">Tram to compare (can be null)</param>
    /// <returns>The CompareTo for which tram should be displayed first</returns>
    public int Compare(Tram aTram, Tram bTram)
    {
        var aTramWait = int.Parse(aTram?.Wait ?? "-1");
        var bTramWait = int.Parse(bTram?.Wait ?? "-1");
        return aTramWait.CompareTo(bTramWait);
    }
}