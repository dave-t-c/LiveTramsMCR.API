using System;
using System.IO;
using LiveTramsMCR.Models.Resources;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.TestResources;

/// <summary>
///     Tests the StopsLoader class.
///     The StopsLoader class is used for removing the importing of stops and stop data
///     from the StopsController.
/// </summary>
public class TestStopsLoader
{
    private const string StopResourcePathConst = "../../../Resources/ValidStopLoader.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string InvalidFilePath = "../../../Resources/NonExistentFile.json";
    private ResourcesConfig? _invalidStopResources;
    private ResourcesConfig? _nullStopResource;
    private ResourcesConfig? _validResourcesConfig;


    /// <summary>
    ///     Create the required Configs for the StopLoaders.
    ///     This uses a DeepCopy function created in the ResourceConfig class,
    ///     that then utilises the valid config and changes the fields accordingly.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        //The links for the resources folder is three directories up due to where
        //the tests are run from.
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath
        };

        _invalidStopResources = _validResourcesConfig.DeepCopy();
        _invalidStopResources.StopResourcePath = InvalidFilePath;

        _nullStopResource = _validResourcesConfig.DeepCopy();
        _nullStopResource.StopResourcePath = null;
    }

    /// <summary>
    ///     Set the created configs to null to ensure there is no carry through
    ///     between tests.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _validResourcesConfig = null;
        _invalidStopResources = null;
        _nullStopResource = null;
    }

    /// <summary>
    ///     Create a valid stop loader.
    ///     This should only contain a single stop.
    ///     This uses the Resources/ValidStopLoader.json file.
    /// </summary>
    [Test]
    public void TestValidStopLoader()
    {
        var testStopLoader = new StopLoader(_validResourcesConfig);
        Assert.AreEqual(1, testStopLoader.ImportStops().Count);
    }

    /// <summary>
    ///     Create a stop loader using null.
    ///     This should throw an argument exception
    /// </summary>
    [Test]
    public void TestNullStopLoader()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'resourcesConfig')"),
            delegate
            {
                var unused = new StopLoader(null);
            });
    }

    /// <summary>
    ///     Create a StopLoader using a file path that does not exist.
    ///     This should throw a FileNotFoundException
    /// </summary>
    [Test]
    public void TestNonExistentFileStopLoader()
    {
        Assert.Throws(Is.TypeOf<FileNotFoundException>()
                .And.Message.Contains("Could not find file")
                .And.Message.Contains("../../../Resources/NonExistentFile.json"),
            delegate
            {
                var stopLoader = new StopLoader(_invalidStopResources);
                stopLoader.ImportStops();
            });
    }

    /// <summary>
    ///     Create a config with stops resource path as null.
    ///     This should throw a InvalidOperationException.
    /// </summary>
    [Test]
    public void TestNullStopsResource()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo("StopResourcePath cannot be null"),
            delegate
            {
                var unused = new StopLoader(_nullStopResource);
            });
    }
}