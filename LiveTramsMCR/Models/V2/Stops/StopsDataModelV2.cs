using System.Collections.Generic;
using LiveTramsMCR.Models.V2.Stops.Data;

namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
/// Data model for processing stops related
/// requests.
/// </summary>
public class StopsDataModelV2: IStopsDataModelV2
{
    private readonly IStopsRepositoryV2 _stopsRepository;

    /// <summary>
    /// Creates a new StopsDataModel using a stops repository to query
    /// for stop info.
    /// </summary>
    /// <param name="stopsRepository">Stops repository for retrieving stop info</param>
    public StopsDataModelV2(IStopsRepositoryV2 stopsRepository)
    {
        _stopsRepository = stopsRepository;
    }
    
    /// <summary>
    /// Returns the stops from the imported resources injected
    /// into the stops model.
    /// </summary>
    /// <returns></returns>
    public List<StopV2> GetStops()
    {
        return _stopsRepository.GetAll();
    }
}