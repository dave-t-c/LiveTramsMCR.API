namespace LiveTramsMCR.Models.V1.Services;

/// <summary>
///     Interface that should be implemented by all classes
///     wanting to act as a data model for Services.
/// </summary>
public interface IServicesDataModel
{
    /// <summary>
    ///     Returns the formatted services for a given stop.
    /// </summary>
    /// <param name="stop">Stop TLAREF for stop</param>
    /// <returns>Services at the specified stop</returns>
    public FormattedServices RequestServices(string stop);

    /// <summary>
    ///     Returns services formatted for a departure board
    /// </summary>
    /// <param name="stop">Stop TLAREF</param>
    /// <returns>Services ordered by increasing departure time for the specified stop</returns>
    public FormattedDepartureBoardServices RequestDepartureBoardServices(string stop);
}