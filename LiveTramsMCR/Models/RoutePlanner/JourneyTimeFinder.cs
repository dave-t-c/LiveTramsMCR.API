using System;

namespace LiveTramsMCR.Models.RoutePlanner;

/// <summary>
/// Finds journey times by identifying the time spent on each route.
/// </summary>
public class JourneyTimeFinder
{
    private readonly RouteTimes _routeTimes;
    
    /// <summary>
    /// Create a new journey time finder with the given route times.
    /// The route times are used to identify times between stops on routes.
    /// </summary>
    /// <param name="routeTimes"></param>
    public JourneyTimeFinder(RouteTimes routeTimes)
    {
        _routeTimes = routeTimes;
    }

    /// <summary>
    /// Find a journey time in mins between an origin and destination stop on a given route.
    /// </summary>
    /// <param name="routeName">Route the journey occurs on</param>
    /// <param name="originStopName">Start of the journey on the route</param>
    /// <param name="destStopName">End of journey on the route.</param>
    /// <returns></returns>
    public int FindJourneyTime(string routeName, string originStopName, string destStopName)
    {
        var selectedRoute = _routeTimes.GetRouteTimes(routeName);
        if (originStopName is null)
            throw new ArgumentNullException(nameof(originStopName));
        if (destStopName is null)
            throw new ArgumentNullException(nameof(destStopName));
        
        if (!selectedRoute.ContainsKey(originStopName))
            throw new InvalidOperationException($"The origin stop '{originStopName}' was not " +
                                                $"found on the '{routeName}' route");
        var originTimeSpan = selectedRoute[originStopName];
            
        if(!selectedRoute.ContainsKey(destStopName))
            throw new InvalidOperationException($"The destination stop '{destStopName}' was not " +
                                                $"found on the '{routeName}' route");
        var destinationTimeSpan = selectedRoute[destStopName];
        var minutesDifference = Math.Abs(originTimeSpan.Subtract(destinationTimeSpan).TotalMinutes);
        return Convert.ToInt32(minutesDifference);
    }
}