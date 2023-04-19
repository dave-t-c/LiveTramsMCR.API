using System.Collections.Generic;
using System.Linq;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Stores services by destination, ordered by ascending wait time.
/// </summary>
public class FormattedServices
{
    /// <summary>
    /// Creates a new, empty, formatted services object.
    /// </summary>
    public FormattedServices()
    {
        InternalDestinations = new Dictionary<string, SortedSet<Tram>>();
        Messages = new HashSet<string>();
        LastUpdated = "";
    }

    /// <summary>
    /// Dict between destination and a sorted set of trams for that dest
    /// </summary>
    public Dictionary<string, SortedSet<Tram>> Destinations
    {
        get
        {
            return InternalDestinations.OrderBy(dest => int.Parse(dest.Value.First().Wait))
                .ToDictionary(dest => dest.Key, dest => dest.Value);

        }
    }

    /// <summary>
    /// Destinations added internally to the class.
    /// These are then ordered on the get of destinations
    /// </summary>
    private Dictionary<string, SortedSet<Tram>> InternalDestinations { get; }

    /// <summary>
    /// Service messages for the 
    /// </summary>
    public HashSet<string> Messages { get; }
    
    /// <summary>
    /// UTC Time string of when the service information was last updated
    /// </summary>
    public string LastUpdated { get; private set; }

    /// <summary>
    ///     Adds a tram to the formatted services.
    ///     This will add it to a set for trams with the same destination.
    /// </summary>
    /// <param name="tram">Tram service to add</param>
    public void AddService(Tram tram)
    {
        if (tram == null) return;
        if (!InternalDestinations.ContainsKey(tram.Destination))
            InternalDestinations[tram.Destination] = new SortedSet<Tram>(new TramComparer());

        InternalDestinations[tram.Destination].Add(tram);
    }

    /// <summary>
    /// Adds a message to the messages for the stop
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(string message)
    {
        if (message == null) return;
        Messages.Add(message);
    }

    /// <summary>
    /// Update the last updated value
    /// This is not updated after initially being set
    /// </summary>
    /// <param name="lastUpdated"></param>
    public void SetLastUpdated(string lastUpdated)
    {
        if (lastUpdated == null) return;
        LastUpdated = lastUpdated;
    }
}