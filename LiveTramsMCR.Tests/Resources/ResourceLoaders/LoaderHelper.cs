using System;
using System.IO;
using static System.AppDomain;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
///     Helper functions used when loading resources into the application.
/// </summary>
public class LoaderHelper
{
    /// <summary>
    ///     Checks the file requirements for a given file in a ResourceConfig
    /// </summary>
    /// <param name="filePath">File path to verify</param>
    /// <param name="argName">Name of the field associated with the file path</param>
    /// <returns>string - full file path for the resource.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when a null file path is given. StopLoaders cannot be created with null resource paths.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///     Thrown when the file cannot be found from the base directory.
    /// </exception>
    public static string CheckFileRequirements(string? filePath, string argName)
    {
        if (filePath is null)
            throw new InvalidOperationException(argName + " cannot be null");
        // The below line has been added as when check file requirements is called again, it tries to run over
        // the already created file path. This likely requires a design improvement
        if (filePath[0] == '/' || filePath[0] == '\\') return filePath;

        filePath = CurrentDomain.BaseDirectory + filePath;
        if (!File.Exists(filePath)) throw new FileNotFoundException("Could not find file " + filePath);
        return filePath;
    }
}