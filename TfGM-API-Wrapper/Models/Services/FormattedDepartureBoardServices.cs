using System;
using System.Collections.Generic;

namespace TfGM_API_Wrapper.Models.Services;

/// <summary>
///     Stores services by destination, ordered by ascending wait time.
/// </summary>
public class FormattedDepartureBoardServices
{
    /// <summary>
    /// Creates a new, empty, formatted services object.
    /// </summary>
    public FormattedDepartureBoardServices()
    {
        Trams = new SortedSet<Tram>();
        Messages = new HashSet<string>();
    }

    /// <summary>
    /// Set of trams ordered by increasing arrival time
    /// </summary>
    public SortedSet<Tram> Trams { get; }
    
    /// <summary>
    /// Service messages for the 
    /// </summary>
    public HashSet<string> Messages { get; }

    /// <summary>
    ///     Adds a tram to the formatted services.
    /// </summary>
    /// <param name="tram">Tram service to add</param>
    public void AddService(Tram tram)
    {
        this.Trams.Add(new Tram(
            "Example Destination",
            "Single",
            "Due",
            "9"
            ));
    }

    /// <summary>
    /// Adds a message to the messages for the stop
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(string message)
    {
        throw new NotImplementedException();
    }
}