using System;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
///     Finds journey times by identifying the time spent on each route.
/// </summary>
public class JourneyTimeFinder
{
    private readonly IRouteRepository _routeRepository;

    /// <summary>
    ///     Create a new journey time finder with the given route times.
    ///     The route times are used to identify times between stops on routes.
    /// </summary>
    /// <param name="routeRepository"></param>
    public JourneyTimeFinder(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    /// <summary>
    ///     Find a journey time in mins between an origin and destination stop on a given route.
    /// </summary>
    /// <param name="routeName">Route the journey occurs on</param>
    /// <param name="originStopName">Start of the journey on the route</param>
    /// <param name="destStopName">End of journey on the route.</param>
    /// <returns></returns>
    public int FindJourneyTime(string routeName, string originStopName, string destStopName)
    {
        if (routeName is null)
            throw new ArgumentNullException(nameof(routeName));

        var selectedRoute = _routeRepository.GetRouteTimesByNameAsync(routeName);

        if (selectedRoute is null)
            throw new ArgumentException($"The route '{routeName}' was not found");

        if (originStopName is null)
            throw new ArgumentNullException(nameof(originStopName));
        if (destStopName is null)
            throw new ArgumentNullException(nameof(destStopName));

        if (!selectedRoute!.Times.ContainsKey(originStopName))
            throw new InvalidOperationException($"The origin stop '{originStopName}' was not " +
                                                $"found on the '{routeName}' route");
        var originTimeSpan = selectedRoute.Times[originStopName];

        if (!selectedRoute.Times.ContainsKey(destStopName))
            throw new InvalidOperationException($"The destination stop '{destStopName}' was not " +
                                                $"found on the '{routeName}' route");
        var destinationTimeSpan = selectedRoute.Times[destStopName];
        var minutesDifference = Math.Abs(originTimeSpan.Subtract(destinationTimeSpan).TotalMinutes);
        return Convert.ToInt32(minutesDifference);
    }
}