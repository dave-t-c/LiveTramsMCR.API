using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;

/// <inheritdoc />
public class NextServiceIdentifierV2 : INextServiceIdentifierV2
{

    /// <inheritdoc />
    public NextServiceIdentifierV2Response IdentifyNextService(NextServiceIdentifierV2Request request)
    {
        var originStop = request.Origin;
        var destinationStop = request.Destination;
        var routes = request.Routes;
        var services = request.Services;
        
        // Construct stop keys for looking up stops on routes
        var originStopKey = new StopKeysV2()
        {
            StopName = originStop.StopName, Tlaref = originStop.Tlaref
        };

        var destinationStopKey = new StopKeysV2()
        {
            StopName = destinationStop.StopName, Tlaref = destinationStop.Tlaref
        };

        SetExpectedDestinationForViaServices(services);
        
        var minWaitDict = new Dictionary<StopKeysV2, int>();
        
        // Outline
        // For each route
        foreach (var route in routes)
        {
            // Identify the direction of the route (e.g. index is increasing / decreasing)
            var increment = IdentifyRouteDirection(originStopKey, destinationStopKey, route);

            // Identify the stops on the route that are the destination or after it.
            var possibleDestinationStops = IdentifyDestinationStops(destinationStopKey, route, increment);
            
            // Identify destinations 
            var destinations = IdentifyDestinationsFromServices(services);

            // Check the destinations of the services to identify if there are a match
            var matchedDestinations = IdentifyMatchedDestinations(destinations, possibleDestinationStops);
            
            // Identify the minimum time for each matched destination
            var minWaitForMatchedDestinations = IdentifyMinimumWaitTimeForDestination(matchedDestinations, services);

            foreach (var destination in minWaitForMatchedDestinations)
            {
                if (!minWaitDict.TryGetValue(destination.Key, out var existingValue))
                {
                    minWaitDict[destination.Key] = destination.Value;
                }
                else
                {
                    minWaitDict[destination.Key] = Math.Min(existingValue, destination.Value);    
                }
                
            }
        }

        if (minWaitDict.Count == 0)
        {
            return null;
        }
        
        // Check the minimum time for the destinations for each route, return the route with the smallest.
        var minDestination = minWaitDict.MinBy(kvp => kvp.Value);

        return new NextServiceIdentifierV2Response()
        {
            Destination = minDestination.Key, Wait = minDestination.Value
        };
    }

    private static void SetExpectedDestinationForViaServices(List<Tram> services)
    {
        foreach (var service in services)
        {
            if (!service.Destination.Contains("via")) continue;
            
            var originalDestination = service.Destination;
            var finalDestination = originalDestination.Split("via")[0].Trim();
            service.Destination = finalDestination;
        }
    }

    private static int IdentifyRouteDirection(StopKeysV2 originStopKey, StopKeysV2 destinationStopKey, RouteV2 route)
    {
        var originStopIndex = route.Stops.IndexOf(originStopKey);
        var destinationStopIndex = route.Stops.IndexOf(destinationStopKey);
        var increment = destinationStopIndex > originStopIndex ? 1 : -1;
        return increment;
    }

    private static IEnumerable<StopKeysV2> IdentifyDestinationStops(StopKeysV2 destinationStopKey, RouteV2 route, int increment)
    {
        var destinationIndex = route.Stops.IndexOf(destinationStopKey);
        IEnumerable<StopKeysV2> identifiedStops;
        
        // If the increment is 1, we want all of the items from the destination index and after
        // Otherwise we want the ones up to and including the destination index.
        if (increment == 1)
        {
            identifiedStops = route.Stops.Where((stop, index) => index >= destinationIndex);
        }
        else
        {
            identifiedStops = route.Stops.Where((stop, index) => index <= destinationIndex);
        }

        return identifiedStops;
    }
    
    private static IEnumerable<string> IdentifyDestinationsFromServices(IEnumerable<Tram> services)
    {
        var destinations = services.Select(service => service.Destination).ToList();
        return destinations;
    }

    private static IEnumerable<StopKeysV2> IdentifyMatchedDestinations(IEnumerable<string> destinations,
        IEnumerable<StopKeysV2> possibleDestinations)
    {
        var matchedDestinations = possibleDestinations.Where(possibleDestination =>
            destinations.Contains(possibleDestination.StopName));
        return matchedDestinations;
    }

    private static Dictionary<StopKeysV2, int> IdentifyMinimumWaitTimeForDestination(IEnumerable<StopKeysV2> matchedDestinations,
        List<Tram> services)
    {
        var minimumWaitDict = new Dictionary<StopKeysV2, int>();

        foreach (var destination in matchedDestinations)
        {
            var servicesToDestination = services.Where(
                service => service.Destination == destination.StopName);
            var serviceWaits = servicesToDestination.Select(service => int.Parse(service.Wait));
            var minWait = serviceWaits.Min();
            
            minimumWaitDict.Add(destination, minWait);
        }

        return minimumWaitDict;
    }
}