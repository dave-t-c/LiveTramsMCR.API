// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
/// Stores unformatted service information, which is the result of a request to the TfGM API.
/// This uses the result of a request for a single ID.
/// Each UnformattedServices object can potentially store at most 3 Trams. 
/// </summary>
public class UnformattedServices
{
    /// <summary>
    /// ID used in TfGM API request
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Line the ID and stop belong to.
    /// This is a single line, so if a stop is on multiple lines,
    /// it will only show one.
    /// This does not show the stop colour, e.g. purple.
    /// </summary>
    public string Line { get; set; }
    
    /// <summary>
    /// 3 character code for a stop, e.g.
    /// for Piccadilly, PIC.
    /// <para>
    /// Please note, Deansgate - Castlefield uses Tlaref 'GMX' in the existing version of the TfGM API
    /// </para>
    /// </summary>
    public string Tlaref { get; set; }
    
    /// <summary>
    /// Reference to the Passenger Information Display (PID), for the request
    /// </summary>
    public string Pidref { get; set; }
    
    /// <summary>
    /// Name of the station the ID is located at, e.g. Piccadilly
    /// </summary>
    public string StationLocation { get; set; }
    
    /// <summary>
    /// Naptan code for the station
    /// </summary>
    public string AtcoCode { get; set; }
    
    /// <summary>
    /// Direction of tram, can either be 'Incoming' - heading towards the city centre,
    /// or 'Outgoing - heading out of the city.
    /// <para>
    /// N.B. On the 2CC Incoming / Outgoing changes between Exchange Square and St Peter's Square
    /// On the 1CC, Incoming and Outgoing changes at Delta junction, the junction between St Peter's Square
    /// and Piccadilly Gardens / Market Street.
    /// </para>
    /// </summary>
    public string Direction { get; set; }
    
    /// <summary>
    /// Destination for the first tram
    /// </summary>
    public string Dest0 { get; set; }
    
    /// <summary>
    /// Carriages for the first Tram, either 'Single' or 'Double'
    /// </summary>
    public string Carriages0 { get; set; }
    
    /// <summary>
    /// Status for the first Tram, such as 'Due' or 'Arrived'
    /// </summary>
    public string Status0 { get; set; }
    
    /// <summary>
    /// Wait for the first Tram, an int of minutes.
    /// </summary>
    public string Wait0 { get; set; }
    
    /// <summary>
    /// Destination for the second Tram
    /// </summary>
    public string Dest1 { get; set; }
    
    /// <summary>
    /// Carriages for the second Tram
    /// </summary>
    public string Carriages1 { get; set; }
    /// <summary>
    /// Status for the second Tram
    /// </summary>
    public string Status1 { get; set; }
    
    /// <summary>
    /// Wait for the second Tram
    /// </summary>
    public string Wait1 { get; set; }
    
    /// <summary>
    /// Destination for the third Tram
    /// </summary>
    public string Dest2 { get; set; }
    
    /// <summary>
    /// Carriages for the third Tram
    /// </summary>
    public string Carriages2 { get; set; }
    
    /// <summary>
    /// Status for the third Tram
    /// </summary>
    public string Status2 { get; set; }
    
    /// <summary>
    /// Wait for the third Tram
    /// </summary>
    public string Wait2 { get; set; }
    
    /// <summary>
    /// Destination for the fourth Tram
    /// </summary>
    public string Dest3 { get; set; }
    
    /// <summary>
    /// Carriages for the fourth Tram
    /// </summary>
    public string Carriages3 { get; set; }
    
    /// <summary>
    /// Status for the fourth Tram
    /// </summary>
    public string Status3 { get; set; }
    
    /// <summary>
    /// Wait for the fourth Tram
    /// </summary>
    public string Wait3 { get; set; }
    
    /// <summary>
    /// Messages displayed on the Passenger Information Display, e.g. about disruption or events.
    /// <para>
    /// If there is no message being displayed, this field will be "&lt;no message&gt;"
    /// </para>
    /// </summary>
    public string MessageBoard { get; set; }
    
    /// <summary>
    /// String DateTime of when the request was made.
    /// </summary>
    public string LastUpdated { get; set; }
}

/// <summary>
/// Stores multiple unformatted services together.
/// This can be used when the unformatted services for more than one ID is requested, e.g. for
/// when no ID is specified.
/// </summary>
public class MultipleUnformattedServices
{
    /// <summary>
    /// List of unformatted services returned
    /// </summary>
    [JsonPropertyName("value")]
    public List<UnformattedServices> Value { get; set; }
}