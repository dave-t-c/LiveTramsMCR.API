using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LiveTramsMCR.Models.Resources;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.TestResources;

/// <summary>
///     Tests the StationNamesToTlarefLoader.
/// </summary>
public class TestStationNamesToTlarefLoader
{
    private const string StopResourcePathConst = "../../../Resources/ValidStopLoader.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string InvalidFilePath = "../../../Resources/NonExistentFile.json";
    private ResourcesConfig? _invalidStationNamesToTlarefs;
    private ResourcesConfig? _nullStationNamesToTlarefs;
    private ResourcesConfig? _validResourcesConfig;

    private StationNamesToTlarefLoader? _validStationNamesToTlarefLoader;

    /// <summary>
    ///     Sets up the required ResourceConfigs.
    ///     A valid config is created, and then modified copies with incorrect values are created using a
    ///     deep copy of the valid config.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath
        };

        _validStationNamesToTlarefLoader = new StationNamesToTlarefLoader(_validResourcesConfig);

        _nullStationNamesToTlarefs = _validResourcesConfig.DeepCopy();
        _nullStationNamesToTlarefs.StationNamesToTlarefsPath = null;

        _invalidStationNamesToTlarefs = _validResourcesConfig.DeepCopy();
        _invalidStationNamesToTlarefs.StationNamesToTlarefsPath = InvalidFilePath;
    }

    /// <summary>
    ///     Clear the created resources.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
        _nullStationNamesToTlarefs = null;
        _invalidStationNamesToTlarefs = null;
        _validStationNamesToTlarefLoader = null;
    }

    /// <summary>
    ///     Test to try and create a StopLoader with a null
    ///     StationNamesToTlarefs path.
    ///     This should throw an Invalid Operation Exception similar to with a null
    ///     StopResourcePath.
    /// </summary>
    [Test]
    public void TestNullStationNamesToTlarefs()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("StationNamesToTlarefsPath cannot be null"),
            delegate { var unused = new StationNamesToTlarefLoader(_nullStationNamesToTlarefs); });
    }

    /// <summary>
    ///     Test to try and create a file with a non-existent StationNamesToTlarefs
    ///     path. This should throw a FileNotFoundException.
    /// </summary>
    [Test]
    public void TestNonExistentStationNamesToTlarefs()
    {
        Assert.Throws(Is.TypeOf<FileNotFoundException>()
                .And.Message.Contains("Could not find file")
                .And.Message.Contains("../../../Resources/NonExistentFile.json"),
            delegate { var unused = new StationNamesToTlarefLoader(_invalidStationNamesToTlarefs); });
    }

    /// <summary>
    ///     Test to try and import the station names to tlaref dict.
    ///     This should return a dictionary that contains 12 entries.
    /// </summary>
    [Test]
    public void TestImportStationNamesDict()
    {
        Dictionary<string, string>? result = _validStationNamesToTlarefLoader?.ImportStationNamesToTlarefs();
        Debug.Assert(result != null, nameof(result) + " != null");
        Assert.NotNull(result);
        Assert.AreEqual(12, result.Count);
        Assert.AreEqual("ALT", result["Altrincham"]);
    }
}