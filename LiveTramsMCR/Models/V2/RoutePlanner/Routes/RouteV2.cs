using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Geolocation;
using LiveTramsMCR.Models.V2.Stops;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static LiveTramsMCR.Configuration.AppConfiguration;

namespace LiveTramsMCR.Models.V2.RoutePlanner.Routes;

/// <summary>
///     Represents route information.
/// </summary>
[BsonIgnoreExtraElements]
public class RouteV2
{

    /// <summary>
    ///     Object ID used by mongodb
    /// </summary>
    [JsonIgnore]
    public ObjectId Id { get; set; }

    /// <summary>
    ///     Name of the route, e.g. "Purple"
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Hex colour string for the route, e.g. #7B2082
    /// </summary>
    public string Colour { get; set; }

    /// <summary>
    ///     Stops belonging to a route in the order they can be travelled between.
    /// </summary>
    public List<StopKeysV2> Stops { get; set; }

#nullable enable
    /// <summary>
    ///     Stop detail generated using the Stop keys
    ///     Contains the full detail for all stops on the route.
    /// </summary>
    public List<StopV2>? StopsDetail { get; set; }
#nullable disable

    /// <summary>
    ///     List of co-ordinates to create a polyline for the route.
    ///     This follows the same direction as the stops.
    ///     In the format longitude, latitude
    /// </summary>
    public List<List<double>> PolylineCoordinates { get; set; }

    /// <summary>
    ///     Identifies Stops between an Origin and Destination stop.
    ///     This excludes the origin and destination stop.
    /// </summary>
    /// <param name="origin">Stop to start at.</param>
    /// <param name="destination">Stop to end at.</param>
    /// <returns>List of intermediate stops between origin and destination on given route.</returns>
    public List<StopV2> GetIntermediateStops(StopKeysV2 origin, StopKeysV2 destination)
    {
        ValidateStopsOnRoute(origin, destination);

        var originIndex = Stops.IndexOf(origin);
        var destinationIndex = Stops.IndexOf(destination);
        var increment = destinationIndex > originIndex ? 1 : -1;

        var intermediateStops = new List<StopV2>();
        for (var i = originIndex + increment; i != destinationIndex; i += increment)
        {
            intermediateStops.Add(StopsDetail?[i]);
        }

        return intermediateStops;
    }

    /// <summary>
    ///     Validates the stops being used on the given route.
    ///     This returns true if the stops are both valid and exist on the provided route.
    /// </summary>
    /// <param name="origin">Origin Stop to validate</param>
    /// <param name="destination">Destination Stop to validate</param>
    /// <exception cref="ArgumentNullException">Thrown if any argument is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if either of the stops do not exist on the provided route</exception>
    public void ValidateStopsOnRoute(StopKeysV2 origin, StopKeysV2 destination)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));

        if (!Stops.Contains(origin))
            throw new InvalidOperationException(origin.StopName + " does not exist on " + Name + " route");
        if (!Stops.Contains(destination))
            throw new InvalidOperationException(destination.StopName + " does not exist on " + Name + " route");
    }

    /// <summary>
    /// Gets the polyline between certain stops on a route.
    /// </summary>
    /// <param name="origin">Start of polyline</param>
    /// <param name="destination">Stop on end of polyline</param>
    /// <returns></returns>
    public List<List<double>> GetPolylineBetweenStops(StopKeysV2 origin, StopKeysV2 destination)
    {
        ValidateStopsOnRoute(origin, destination);

        if (StopsDetail is null)
            throw new InvalidOperationException("Cannot find polyline when stop detail is null");
        
        var originStopDetail = StopsDetail?[Stops.IndexOf(origin)];
        var destinationStopDetail = StopsDetail?[Stops.IndexOf(destination)];
        
        var polylineCoordinates = PolylineCoordinates.Select(pc =>
            new Coordinate(pc[1], pc[0])).ToList();

        var originStopCoordinate = new Coordinate(originStopDetail!.Latitude, originStopDetail!.Longitude);
        var destinationStopCoordinate = new Coordinate(destinationStopDetail!.Latitude, destinationStopDetail!.Longitude);

        var closestCoordinateToOrigin = polylineCoordinates.MinBy(pc =>
            GeoCalculator.GetDistance(pc, originStopCoordinate));

        var closestCoordinateToDestination = polylineCoordinates.MinBy(pc =>
            GeoCalculator.GetDistance(pc, destinationStopCoordinate));

        var closestOriginCoordinateIndex = IdentifyClosestCoordinateIndex(closestCoordinateToOrigin);

        var closestDestinationCoordinateIndex = IdentifyClosestCoordinateIndex(closestCoordinateToDestination);
        
        // Need to check which should be the start of the range and then how many items to take
        // Closest to origin index is not guaranteed to be lower than destination index, could be other way around.

        var lowIndex = Math.Min(closestOriginCoordinateIndex, closestDestinationCoordinateIndex);
        var countIncludingFinalCoordinate = Math.Abs(closestOriginCoordinateIndex - closestDestinationCoordinateIndex) + 1;

        return PolylineCoordinates.Skip(lowIndex).Take(countIncludingFinalCoordinate).ToList();
    }

    /// <summary>
    /// Identifies the closest index to a coordinate on the routes polyline.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns>Index of closest stop within tolerance, -1 if not found.</returns>
    private int IdentifyClosestCoordinateIndex(Coordinate coordinate)
    {
        return PolylineCoordinates.FindIndex(coord =>
            Math.Abs(coord[1] - coordinate.Latitude) < LocationAccuracyTolerance && 
            Math.Abs(coord[0] - coordinate.Longitude) < LocationAccuracyTolerance
        );
    }
}