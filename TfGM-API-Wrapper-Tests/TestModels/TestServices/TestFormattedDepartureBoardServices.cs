using System.Linq;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Services;

namespace TfGM_API_Wrapper_Tests.TestModels.TestServices;

/// <summary>
/// Test class for FormattedDepartureBoardServices
/// </summary>
public class TestFormattedDepartureBoardServices
{
    private string? _carriages;
    private string? _destination;
    private string? _diffCarriages;
    private string? _diffDestination;
    private string? _diffWait;
    private FormattedDepartureBoardServices? _formattedDepartureBoardServices;
    private string? _status;
    private Tram? _tram;
    private Tram? _diffTram;
    private Tram? _tramDiffDestination;
    private Tram? _tramSameDestinationDiffWait;
    private string? _wait;
    
    [SetUp]
    public void SetUp()
    {
        _destination = "Example Destination";
        _diffDestination = "Different Destination";
        _carriages = "Single";
        _diffCarriages = "Double";
        _status = "Due";
        _wait = "9";
        _diffWait = "1";
        _tram = new Tram(_destination, _carriages, _status, _wait);
        _tramDiffDestination = new Tram(_diffDestination, _carriages, _status, _wait);
        _tramSameDestinationDiffWait = new Tram(_destination, _diffCarriages, _status, _diffWait);
        _formattedDepartureBoardServices = new FormattedDepartureBoardServices();
        _diffTram = new Tram(_diffDestination, _diffCarriages, _status, _diffWait);
    }

    [TearDown]
    public void TearDown()
    {
        _destination = null;
        _diffDestination = null;
        _carriages = null;
        _diffCarriages = null;
        _status = null;
        _wait = null;
        _diffWait = null;
        _tram = null;
        _tramDiffDestination = null;
        _tramSameDestinationDiffWait = null;
        _formattedDepartureBoardServices = null;
    }


    /// <summary>
    /// Test to add a single tram
    /// Set should contain a single item, matching the destination.
    /// </summary>
    [Test]
    public void TestAddSingleTram()
    {
        _formattedDepartureBoardServices?.AddService(_tram);
        var returnedServices = _formattedDepartureBoardServices?.Trams;
        Assert.IsNotNull(returnedServices);
        Assert.AreEqual(1, returnedServices?.Count);
        var returnedTram = returnedServices?.First();
        Assert.AreEqual("Example Destination", returnedTram?.Destination);
        Assert.AreEqual("9", returnedTram?.Wait);
        Assert.AreEqual("Single", returnedTram?.Carriages);
        Assert.AreEqual("Due", returnedTram?.Status);
    }


    /// <summary>
    /// Test to add a different tram
    /// This should match the added values.
    /// </summary>
    [Test]
    public void TestAddDifferentTram()
    {
        _formattedDepartureBoardServices?.AddService(_diffTram);
        var returnedServices = _formattedDepartureBoardServices?.Trams;
        Assert.IsNotNull(returnedServices);
        Assert.AreEqual(1, returnedServices?.Count);
        var returnedTram = returnedServices?.First();
        Assert.AreEqual(_diffDestination, returnedTram?.Destination);
        Assert.AreEqual(_diffWait, returnedTram?.Wait);
        Assert.AreEqual(_diffCarriages, returnedTram?.Carriages);
        Assert.AreEqual("Due", returnedTram?.Status);
    }
}