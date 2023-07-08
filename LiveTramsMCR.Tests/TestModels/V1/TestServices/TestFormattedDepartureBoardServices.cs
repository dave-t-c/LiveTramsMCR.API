using System.Linq;
using LiveTramsMCR.Models.V1.Services;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

/// <summary>
///     Test class for FormattedDepartureBoardServices
/// </summary>
public class TestFormattedDepartureBoardServices
{
    private string? _carriages;
    private string? _destination;
    private string? _diffCarriages;
    private string? _diffDestination;
    private Tram? _diffTram;
    private string? _diffWait;
    private FormattedDepartureBoardServices? _formattedDepartureBoardServices;
    private string? _status;
    private Tram? _tram;
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
        _tramSameDestinationDiffWait = null;
        _formattedDepartureBoardServices = null;
    }


    /// <summary>
    ///     Test to add a single tram
    ///     Set should contain a single item, matching the destination.
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
    ///     Test to add a different tram
    ///     This should match the added values.
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

    /// <summary>
    ///     Test to add a null tram.
    ///     This should not be added to the set and the set size should remain zero.
    /// </summary>
    [Test]
    public void TestAddNullTram()
    {
        _formattedDepartureBoardServices?.AddService(null);
        var returnedServices = _formattedDepartureBoardServices?.Trams;
        Assert.IsNotNull(returnedServices);
        Assert.IsEmpty(returnedServices!);
    }

    /// <summary>
    ///     Test to add multiple trams with different waits.
    ///     This should return a set ordered by the due time.
    /// </summary>
    [Test]
    public void TestAddMultipleTrams()
    {
        _formattedDepartureBoardServices?.AddService(_tram);
        _formattedDepartureBoardServices?.AddService(_tramSameDestinationDiffWait);
        var returnedServices = _formattedDepartureBoardServices?.Trams;
        Assert.IsNotNull(returnedServices);
        Assert.AreEqual(2, returnedServices?.Count);
        Assert.Contains(_tram, returnedServices);
        Assert.Contains(_tramSameDestinationDiffWait, returnedServices);
        Assert.IsTrue(returnedServices?.ElementAt(0).Equals(_tramSameDestinationDiffWait));
        Assert.IsTrue(returnedServices?.ElementAt(1).Equals(_tram));
    }

    /// <summary>
    ///     Test to add a message to the formatted services.
    ///     This should leave a set with size 1.
    /// </summary>
    [Test]
    public void TestAddMessage()
    {
        _formattedDepartureBoardServices?.AddMessage("Example");
        Assert.NotNull(_formattedDepartureBoardServices?.Messages);
        Assert.AreEqual(1, _formattedDepartureBoardServices?.Messages.Count);
        Assert.IsTrue(_formattedDepartureBoardServices?.Messages.Contains("Example"));
    }

    /// <summary>
    ///     Test to add a null message to formatted services.
    ///     This should not be added to the set, and the size should remain 0.
    /// </summary>
    [Test]
    public void TestAddNullMessage()
    {
        _formattedDepartureBoardServices?.AddMessage(null);
        Assert.NotNull(_formattedDepartureBoardServices?.Messages);
        Assert.AreEqual(0, _formattedDepartureBoardServices?.Messages.Count);
    }
}