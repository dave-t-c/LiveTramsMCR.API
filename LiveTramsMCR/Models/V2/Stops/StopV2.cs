using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LiveTramsMCR.Models.V2.RoutePlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using MongoDB.Bson;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
///     Stores information about a single stop.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
public sealed class StopV2 : IEquatable<StopV2>, IEqualityComparer<StopV2>
{
    /// <summary>
    /// Object Id used internally by mongodb.
    /// </summary>
    [JsonIgnore]
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// Name of the stop, such as Piccadilly
    /// </summary>
    public string StopName { get; set; }
    
    /// <summary>
    /// 3 code ID for the stop, e.g. PIC for Piccadilly 
    /// </summary>
    public string Tlaref { get; set; }
    
    /// <summary>
    /// Routes the stop is on.
    /// </summary>
    public List<SimpleRouteV2> Routes { get; set; }
    
    /// <summary>
    /// Naptan ID for the stop. This can be used to look up more information
    /// in government transport data sets
    /// </summary>
    public string AtcoCode { get; set; }
    
    /// <summary>
    /// Stop Latitude. This may be different to that shown by apple or google maps.
    /// </summary>
    public double Latitude { get; set; }
    
    /// <summary>
    /// Stop Longitude. This may be different to that shown by apple or google maps.
    /// </summary>
    public double Longitude { get; set; }
    
    /// <summary>
    /// Street the stop is on. If it is not directly on a street, it will be prefixed
    /// with 'Off'.
    /// </summary>
    public string Street { get; set; }
    
    /// <summary>
    /// Closest road intersection to the stop. For stops where there is not a close intersection,
    /// this will be blank. 
    /// </summary>
    public string RoadCrossing { get; set; }
    
    /// <summary>
    /// Line the stop is on. This is a single value and does not contain all lines.
    /// This will be a destination, such as Bury, and does not include the line colour(s).
    /// </summary>
    public string Line { get; set; }
    
    /// <summary>
    /// Ticket fare zone for the stop. If a stop is in multiple zones, it will
    /// be shown as 'a/b', where a is the smaller of the two zones, e.g. '3/4'.
    /// </summary>
    public string StopZone { get; set; }

    /// <summary>
    /// Checks equality of stops by checking name or tlaref
    /// </summary>
    /// <param name="other">Stop to compare</param>
    /// <returns></returns>
    public bool Equals(StopV2 other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StopName == other.StopName
        || Tlaref == other.Tlaref; 

    }

    /// <summary>
    /// Checks equality between this stop and another obj
    /// </summary>
    /// <param name="obj">Obj to compare</param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((StopV2) obj);
    }
    
    /// <summary>
    /// Generates a hash code for a stop
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(StopName);
        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Compares two stop objects
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(StopV2 x, StopV2 y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.StopName == y.StopName && x.Tlaref == y.Tlaref;
    }

    /// <summary>
    /// Generates a hash code for a stop obj
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(StopV2 obj)
    {
        return HashCode.Combine(obj.StopName, obj.Tlaref);
    }
}