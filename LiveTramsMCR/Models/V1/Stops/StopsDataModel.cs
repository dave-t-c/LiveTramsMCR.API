using System.Collections.Generic;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.Stops.Data;

namespace LiveTramsMCR.Models.V1.Stops;

/// <summary>
/// Data model for processing stops related
/// requests.
/// </summary>
public class StopsDataModel: IStopsDataModel
{
    private readonly IStopsRepository _stopsRepository;

    /// <summary>
    /// Creates a new StopsDataModel using the Injected Imported Resources
    /// </summary>
    /// <param name="stopsRepository">Stops repository for retrieving stop info</param>
    public StopsDataModel(IStopsRepository stopsRepository)
    {
        _stopsRepository = stopsRepository;
    }
    
    /// <summary>
    /// Returns the stops from the imported resources injected
    /// into the stops model.
    /// </summary>
    /// <returns></returns>
    public List<Stop> GetStops()
    {
        return _stopsRepository.GetAll();
    }
}