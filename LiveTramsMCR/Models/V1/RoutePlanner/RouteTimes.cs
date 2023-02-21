using System;
using System.Collections.Generic;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
/// Maps a route name, e.g. Purple to a dictionary of it's stops
/// against an example timetable entry.
/// </summary>
public class RouteTimes
{
    private readonly Dictionary<string, Dictionary<string, TimeSpan>> _routeDict;

    /// <summary>
    /// Creates a new route times object without a dictionary of initial values.
    /// </summary>
    public RouteTimes()
    {
        _routeDict = new Dictionary<string, Dictionary<string, TimeSpan>>();
    }

    /// <summary>
    /// Adds a route name to the route times with a dict between stop names and their
    /// time from an example timetable.
    /// </summary>
    /// <param name="routeName">Name of the route to associate these times with.</param>
    /// <param name="stopsDict">Map of stop names to their date-times from a timetable</param>
    public void AddRoute(string routeName, Dictionary<string, TimeSpan> stopsDict)
    {
        if (routeName is null)
            throw new ArgumentNullException(nameof(routeName));
        if (stopsDict is null)
            throw new ArgumentNullException(nameof(stopsDict));
        _routeDict[routeName] = stopsDict;
    }

    /// <summary>
    /// Returns the route times dictionary for a given route
    /// </summary>
    /// <param name="routeName">Route to find a </param>
    /// <returns></returns>
    public Dictionary<string, TimeSpan> GetRouteTimes(string routeName)
    {
        if (routeName is null)
            throw new ArgumentNullException(nameof(routeName));
        if (!_routeDict.ContainsKey(routeName))
            throw new InvalidOperationException($"The route '{routeName}' was not found");
        return _routeDict[routeName];
    }

    /// <summary>
    /// Returns all added routes' times
    /// </summary>
    /// <returns>Returns a dict containing all added routes</returns>
    public Dictionary<string, Dictionary<string, TimeSpan>> GetAllRoutes()
    {
        return _routeDict;
    }
}