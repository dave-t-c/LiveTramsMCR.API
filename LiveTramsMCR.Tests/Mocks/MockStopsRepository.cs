using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;

namespace LiveTramsMCR.Tests.Mocks;

public class MockStopsRepository : IStopsRepository
{
    private readonly List<Stop> _stops;

    public MockStopsRepository(List<Stop> stops)
    {
        _stops = stops;
    }
    public Stop GetStop(string searchTerm)
    {
        return _stops.Find(stop =>
            stop.StopName.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
            || stop.Tlaref.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
        )!;
    }

    public List<Stop> GetAll()
    {
        return _stops;
    }

    public void UpdateStops(List<Stop> stops)
    {
        foreach (var stop in stops.ToList())
        {
            UpdateStop(stop);
        }
    }

    public void UpdateStop(Stop stop)
    {
        var existing = _stops.Find(s => s.Tlaref == stop.Tlaref);
        var existingIndex = _stops.IndexOf(existing!);
        _stops[existingIndex] = stop;
    }
}