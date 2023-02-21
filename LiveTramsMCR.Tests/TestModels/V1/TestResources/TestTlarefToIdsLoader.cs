using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LiveTramsMCR.Models.V1.Resources;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestResources;

/// <summary>
///     Test class for TlarefToIdsLoader.
///     This class is responsible for importing the Tlarefs to Ids map.
/// </summary>
public class TestTlarefToIdsLoader
{
    private const string StopResourcePathConst = "../../../Resources/ValidStopLoader.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string InvalidFilePath = "../../../Resources/NonExistentFile.json";
    private ResourcesConfig? _invalidTlarefsToIdsPath;
    private ResourcesConfig? _nullTlarefsToIdsPath;

    private TlarefToIdsLoader? _tlarefToIdsLoader;
    private ResourcesConfig? _validResourcesConfig;

    /// <summary>
    ///     Creates the required test configs.
    ///     A valid config is created, with invalid configs created with the required changed
    ///     field using deep copies of the valid config.
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

        _nullTlarefsToIdsPath = _validResourcesConfig.DeepCopy();
        _nullTlarefsToIdsPath.TlarefsToIdsPath = null;

        _invalidTlarefsToIdsPath = _validResourcesConfig.DeepCopy();
        _invalidTlarefsToIdsPath.TlarefsToIdsPath = InvalidFilePath;

        _tlarefToIdsLoader = new TlarefToIdsLoader(_validResourcesConfig);
    }

    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
        _nullTlarefsToIdsPath = null;
        _invalidTlarefsToIdsPath = null;
        _tlarefToIdsLoader = null;
    }

    /// <summary>
    ///     Test to try and create a StopLoader with a null
    ///     TlarefsToIdsPath. This should throw a InvalidOperationException.
    /// </summary>
    [Test]
    public void TestNullTlarefsToIdsPath()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("TlarefsToIdsPath cannot be null"),
            delegate
            {
                var unused = new TlarefToIdsLoader(_nullTlarefsToIdsPath);
            });
    }

    /// <summary>
    ///     Test creating a StopLoader with a non-existent TlarefsToIds file.
    ///     This should throw a file not found exception.
    /// </summary>
    [Test]
    public void TestNonExistentTlarefsToIdsPath()
    {
        Assert.Throws(Is.TypeOf<FileNotFoundException>()
                .And.Message.Contains("Could not find file")
                .And.Message.Contains("../../../Resources/NonExistentFile.json"),
            delegate { var unused = new TlarefToIdsLoader(_invalidTlarefsToIdsPath); });
    }

    /// <summary>
    ///     Test to Import the TlarefsToIds dict.
    ///     This should return a dict of length 14.
    /// </summary>
    [Test]
    public void TestImportTlarefsToIds()
    {
        Dictionary<string, List<int>>? result = _tlarefToIdsLoader?.ImportTlarefsToIds();
        var expectedList = new List<int>
        {
            728,
            729
        };
        Debug.Assert(result != null, nameof(result) + " != null");
        Assert.AreEqual(14, result.Count);
        Assert.AreEqual(expectedList, result["ALT"]);
    }
}