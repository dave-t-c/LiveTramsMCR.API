using System;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Stores service information about a single tram.
/// </summary>
public class Tram
{
    /// <summary>
    ///     Constructs a new tram, throws arg null exception if any of the given
    ///     values are null.
    /// </summary>
    /// <param name="destination">Tram destination</param>
    /// <param name="carriages">Num of carriages, either 'Single' or 'Double'</param>
    /// <param name="status">Status of the tram, e.g 'Due', 'Arrived', 'Departing'</param>
    /// <param name="wait">Wait until tram arrives, usually an int of mins but provided as a string</param>
    /// <param name="tlaref">Tlaref this tram information has been reported at</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Tram(string destination, string carriages, string status, string wait, string tlaref)
    {
        Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        Carriages = carriages ?? throw new ArgumentNullException(nameof(carriages));
        Status = status ?? throw new ArgumentNullException(nameof(status));
        Wait = wait ?? throw new ArgumentNullException(nameof(wait));
        Tlaref = tlaref ?? throw new ArgumentNullException(nameof(tlaref));
    }

    /// <summary>
    ///     Destination for the tram, e.g. Piccadilly.
    /// </summary>
    /// <example>Piccadilly</example>
    public string Destination { get; }

    /// <summary>
    ///     Number of carriages the tram has, either 'Single' or 'Double'
    /// </summary>
    /// <example>Double</example>
    // The carriages could be a good candidate for an enum, but given there are only
    // two possible values, but this may be unnecessary. 
    public string Carriages { get; }

    /// <summary>
    ///     Status of the Tram, e.g. 'Due'
    /// </summary>
    /// <example>Due</example>
    public string Status { get; }

    /// <summary>
    ///     Wait for the tram, an int of mins.
    ///     This is stored as a string as this is the format returned by the TfGM API.
    ///     It is not converted as no calculations are completed using it.
    /// </summary>
    /// <example>10</example>
    public string Wait { get; }
    
    /// <summary>
    /// Tlaref of the stop the information for this tram has been reported at.
    /// </summary>
    public string Tlaref { get; }

    /// <summary>
    ///     Determines if this Tram object and an other object are equal.
    ///     All fields are considered.
    /// </summary>
    /// <param name="obj">Object to compare</param>
    /// <returns>boolean: True if Equal.</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        var tram = (Tram)obj;
        return Destination == tram.Destination &&
               Carriages == tram.Carriages &&
               Status == tram.Status &&
               Wait == tram.Wait;
    }

    /// <summary>
    ///     Returns a Hash Code for this Tram that combines
    ///     Hash Codes for all fields.
    /// </summary>
    /// <returns>int: Hash Code Generated using this Tram object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Destination, Carriages, Status, Wait);
    }
}