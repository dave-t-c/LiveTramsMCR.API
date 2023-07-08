using System;

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
    
    /// <summary>
    /// Determines if two stop keys objects are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool Equals(StopKeysV2 other)
    {
        return StopName == other.StopName && Tlaref == other.Tlaref;
    }

    /// <summary>
    /// Determines equality for a stop keys object and another object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((StopKeysV2)obj);
    }

    /// <summary>
    /// Generates a hash code for a stop keys object
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(StopName, Tlaref);
    }
}