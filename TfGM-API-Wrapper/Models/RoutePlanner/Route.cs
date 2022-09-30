using System;
using System.Collections.Generic;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.RoutePlanner;

/// <summary>
/// Class that represents a tram route between two Stops.
/// The stops are included as the Stop class, so all relevant information is available.
/// </summary>
public class Route
{
    /// <summary>
    /// Name of the route, e.g. "Purple"
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Hex colour string for the route, e.g. #7B2082
    /// </summary>
    public string Colour { get; }
    
    /// <summary>
    /// Stops belonging to a route in the order they can be travelled between.
    /// </summary>
    public List<Stop> Stops { get; }
    
    /// <summary>
    /// Creates a new route, the params are currently not checked.
    /// </summary>
    /// <param name="name">Route name, e.g. Purple</param>
    /// <param name="colour">Route hex colour, e.g. #7B2082</param>
    /// <param name="stops"></param>
    ///
    public Route(string name, string colour, List<Stop> stops)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Colour = colour ?? throw new ArgumentNullException(nameof(colour));
        Stops = stops;
    }
}