using System;
using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;

namespace LiveTramsMCR.Tests.Mocks;

public class MockStopsRepositoryV2 : IStopsRepositoryV2
{
    private readonly List<StopV2> _stops;

    public MockStopsRepositoryV2(List<StopV2> stops)
    {
        _stops = stops;
    }
    public StopV2 GetStop(string searchTerm) =>
        _stops.Find(stop =>
            stop.StopName.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
            || stop.Tlaref.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)
        )!;

    public List<StopV2> GetAll()
    {
        return _stops;
    }
}