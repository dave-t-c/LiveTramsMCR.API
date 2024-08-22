namespace LiveTramsMCR.Models.V2.Stops;

/// <summary>
/// Helper used for looking up a stop
/// </summary>
public interface IStopLookupV2
{
    /// <summary>
    ///     Looks up a stop tlaref or name from a value string.
    /// </summary>
    public StopV2 LookupStop(string value);
}