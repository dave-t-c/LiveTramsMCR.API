namespace LiveTramsMCR.Configuration;

public static partial class AppConfiguration
{
    /// <summary>
    /// Location tolerance used when comparing coordinates.
    /// </summary>
    public static double LocationAccuracyTolerance => 0.000001;
}