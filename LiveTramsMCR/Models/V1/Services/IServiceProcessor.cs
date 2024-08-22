using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;

namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Processes service requests
/// </summary>
public interface IServiceProcessor
{

    /// <summary>
    ///     Returns services for a given stop Tlaref.
    /// </summary>
    /// <param name="stopIdentifier">Stop Id, either name or Tlaref. </param>
    /// <returns>Formatted Services for given stop</returns>
    public FormattedServices RequestServices(string stopIdentifier);

    /// <summary>
    /// Request the services for a planned journey.
    /// This will request the service information for origin and destination stops.
    /// The interchange stop will also be requested if the journey involves
    /// and interchange.
    /// </summary>
    public FormattedServices RequestServices(PlannedJourneyV2 plannedJourneyV2);

    /// <summary>
    ///     Returns services formatted for a departure board
    ///     for a given stop Tlaref.
    /// </summary>
    /// <param name="stopTlaref">Stop tlaref</param>
    /// <returns>Service data formatted for use with a departure board</returns>
    public FormattedDepartureBoardServices RequestDepartureBoardServices(string stopTlaref);
}