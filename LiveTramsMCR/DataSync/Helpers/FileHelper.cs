using System.IO;
using MongoDB.Bson.Serialization;

namespace LiveTramsMCR.DataSync.Helpers;

/// <summary>
/// Static helper for file related tasks
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Imports a json file and deserializes into the required type
    /// </summary>
    /// <param name="filePath">Path to json file</param>
    /// <typeparam name="T">Type to deserialize to</typeparam>
    public static T ImportFromJsonFile<T>(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        var importedFile = File.ReadAllText(filePath);
        var deserializedResult = BsonSerializer.Deserialize<T>(importedFile);
        return deserializedResult;
    }
}