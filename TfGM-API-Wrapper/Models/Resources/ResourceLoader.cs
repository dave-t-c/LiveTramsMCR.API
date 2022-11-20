namespace TfGM_API_Wrapper.Models.Resources;

/// <summary>
///     Loads the file resources for the system using the file loaders.
/// </summary>
public class ResourceLoader
{
    private readonly StationNamesToTlarefLoader _stationNamesToTlarefLoader;
    private readonly StopLoader _stopLoader;
    private readonly TlarefToIdsLoader _tlarefToIdsLoader;
    private readonly ResourcesConfig _config;
    private readonly RouteTimesLoader _routeTimesLoader;

    /// <summary>
    /// Creates a new resources loader using the given resources configuration.
    /// No null checks are performed on the resource config given.
    /// </summary>
    /// <param name="resourcesConfig">Resource config used for importing resources</param>
    public ResourceLoader(ResourcesConfig resourcesConfig)
    {
        _config = resourcesConfig;
        _stopLoader = new StopLoader(resourcesConfig);
        _stationNamesToTlarefLoader = new StationNamesToTlarefLoader(resourcesConfig);
        _tlarefToIdsLoader = new TlarefToIdsLoader(resourcesConfig);
        _routeTimesLoader = new RouteTimesLoader(resourcesConfig);
    }

    /// <summary>
    ///     Loads the system resources into a ImportedResources object.
    /// </summary>
    /// <returns>ImportedResources for the system</returns>
    public ImportedResources ImportResources()
    {
        var importedStops = _stopLoader.ImportStops();
        var routeLoader = new RouteLoader(_config, importedStops);
        return new ImportedResources
        {
            ImportedStops = importedStops,
            StationNamesToTlaref = _stationNamesToTlarefLoader.ImportStationNamesToTlarefs(),
            TlarefsToIds = _tlarefToIdsLoader.ImportTlarefsToIds(),
            ImportedRoutes = routeLoader.ImportRoutes(),
            ImportedRouteTimes = _routeTimesLoader.ImportRouteTimes()
        };
    }
}