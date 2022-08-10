namespace TfGM_API_Wrapper.Models.Services;

/// <summary>
/// Interface that should be implemented by all classes
/// wanting to act as a data model for Services.
/// </summary>
public interface IServicesDataModel
{
    /// <summary>
    /// Returns the formatted services for a given stop.
    /// </summary>
    /// <param name="stop">Stop name or TLAREF for stop</param>
    /// <returns>Services at the specified stop</returns>
    public FormattedServices RequestServices(string stop);
}