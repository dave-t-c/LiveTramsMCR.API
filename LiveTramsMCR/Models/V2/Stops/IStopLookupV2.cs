namespace LiveTramsMCR.Models.V2.Stops;

public interface IStopLookupV2
{
    public StopV2 LookupStop(string value);
}