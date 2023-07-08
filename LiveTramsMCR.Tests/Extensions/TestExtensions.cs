using System;
using LiveTramsMCR.Models.V2.Stops;

namespace LiveTramsMCR.Tests.Extensions;

public static class TestExtensions
{
    public static StopKeysV2 ConvertToStopKeysV2(this StopV2? source)
    {
        if (source is null)
        {
            throw new InvalidOperationException("Unable to convert null stop to stop keys");
        }

        return new StopKeysV2()
        {
            StopName = source.StopName, Tlaref = source.Tlaref
        };
    }
}