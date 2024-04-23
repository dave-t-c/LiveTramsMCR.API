namespace LiveTramsMCR.Configuration;

public static partial class AppConfiguration
{
    /// <summary>
    /// Base Static files folder path
    /// </summary>
    private const string StaticFilesPath = "StaticFiles";
    
    /// <summary>
    /// Static routes json file path
    /// </summary>
    public const string RoutesPath = $"{StaticFilesPath}/Routes/routes.json";
    
    /// <summary>
    /// Static routes v2 json file path
    /// </summary>
    public const string RoutesV2Path = $"{StaticFilesPath}/RoutesV2/routesV2.json";
    
    /// <summary>
    /// Static route times path
    /// </summary>
    public const string RouteTimesPath = $"{StaticFilesPath}/RouteTimes/route-times.json";
    
    /// <summary>
    /// Static stops json file path
    /// </summary>
    public const string StopsPath = $"{StaticFilesPath}/Stops/stops.json";
    
    /// <summary>
    /// Static stopsV2 json file path
    /// </summary>
    public const string StopsV2Path = $"{StaticFilesPath}/StopsV2/stopsV2.json";
}