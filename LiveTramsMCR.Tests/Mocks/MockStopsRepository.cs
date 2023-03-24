using System;
using System.Collections.Generic;
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
    public Stop GetStop(string searchTerm) =>
        _stops.Find(stop =>
            stop.StopName.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
            || stop.Tlaref.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
        )!;

    public List<Stop> GetAll()
    {
        return _stops;
    }

    public void UpdateStops(List<Stop> stops)
    {
        throw new NotImplementedException();
    }

    public void UpdateStop(Stop stop)
    {
        throw new NotImplementedException();
    }
}