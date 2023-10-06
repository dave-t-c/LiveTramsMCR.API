using System.Collections.Generic;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
///     POCO Object used for storing routes as they are imported from the resource JSON
/// </summary>
public class UnprocessedRoute
{
    /// <summary>
    ///     Name of the route, e.g. Purple
    /// </summary>
    public string RouteName { get; set; }

    /// <summary>
    ///     Hex string of the route colour, e.g. "#7B2082"
    /// </summary>
    public string Colour { get; set; }

    /// <summary>
    ///     List of stops on the line, in order from one destination to the other
    /// </summary>
    public List<string> Stops { get; set; }
}