using System.Collections.Generic;

namespace LiveTramsMCR.Models.Services;

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
        Destinations = new Dictionary<string, SortedSet<Tram>>();
        Messages = new HashSet<string>();
    }

    /// <summary>
    /// Dict between destination and a sorted set of trams for that dest
    /// </summary>
    public Dictionary<string, SortedSet<Tram>> Destinations { get; }
    
    /// <summary>
    /// Service messages for the 
    /// </summary>
    public HashSet<string> Messages { get; }

    /// <summary>
    ///     Adds a tram to the formatted services.
    ///     This will add it to a set for trams with the same destination.
    /// </summary>
    /// <param name="tram">Tram service to add</param>
    public void AddService(Tram tram)
    {
        if (tram == null) return;
        if (!Destinations.ContainsKey(tram.Destination))
            Destinations[tram.Destination] = new SortedSet<Tram>(new TramComparer());

        Destinations[tram.Destination].Add(tram);
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
}