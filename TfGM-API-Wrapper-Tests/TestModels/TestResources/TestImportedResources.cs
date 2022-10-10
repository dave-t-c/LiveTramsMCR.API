using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;

namespace TfGM_API_Wrapper_Tests.TestModels.TestResources;

/// <summary>
///     Test class for the ImportedResources class.
/// </summary>
public class TestImportedResources
{
    private ImportedResources? _importedResources;

    [SetUp]
    public void SetUp()
    {
        _importedResources = new ImportedResources();
    }

    [TearDown]
    public void TearDown()
    {
        _importedResources = null;
    }

    /// <summary>
    ///     Test to create a new ImportedResources.
    ///     This should contain an empty List of stops for ImportedStops.
    /// </summary>
    [Test]
    public void TestCreateEmptyImportedStops()
    {
        Assert.IsNotNull(_importedResources?.ImportedStops);
        Assert.AreEqual(0, _importedResources?.ImportedStops.Count);
    }

    /// <summary>
    ///     Test to try and get the StationNamesToTlarefs dict.
    ///     This should not be null and should be empty.
    /// </summary>
    [Test]
    public void TestCreateEmptyStationNamesToTlarefs()
    {
        Assert.IsNotNull(_importedResources?.StationNamesToTlaref);
        Assert.AreEqual(0, _importedResources?.StationNamesToTlaref.Count);
    }

    /// <summary>
    ///     Test to create a new ImportedResources object with an
    ///     empty TlarefsToIds dict.
    ///     This should be empty and not null.
    /// </summary>
    [Test]
    public void TestCreateEmptyTlarefsToIds()
    {
        Assert.IsNotNull(_importedResources?.TlarefsToIds);
        Assert.AreEqual(0, _importedResources?.TlarefsToIds.Count);
    }

    /// <summary>
    /// Test to create a new ImportedResources object
    /// with an empty imported routes.
    /// This should not be null and the routes list should be empty.
    /// </summary>
    [Test]
    public void TestCreateEmptyImportedRoutes()
    {
        Assert.IsNotNull(_importedResources?.ImportedRoutes);
        Assert.AreEqual(0, _importedResources?.ImportedRoutes.Count);
    }
}