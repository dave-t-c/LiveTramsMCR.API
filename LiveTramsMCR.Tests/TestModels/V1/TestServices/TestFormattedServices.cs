using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiveTramsMCR.Models.V1.Services;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

public class TestFormattedServices
{
    private string? _carriages;
    private string? _destination;
    private string? _diffCarriages;
    private string? _diffDestination;
    private string? _diffWait;
    private FormattedServices? _formattedServices;
    private string? _status;
    private Tram? _tram;
    private Tram? _tramDiffDestination;
    private Tram? _tramSameDestinationDiffWait;
    private Tram? _tramDiffDestinationDiffWait;
    private string? _wait;
    private const string ExampleLastUpdated = "2023-04-19T19:34:03Z";
    private const string DiffExampleLastUpdated = "2023-04-19T11:34:03Z";

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
        _tramDiffDestinationDiffWait = new Tram(_diffDestination, _diffCarriages, _status, _diffWait);
        _formattedServices = new FormattedServices();
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
        _formattedServices = null;
    }

    /// <summary>
    ///     Test to add a tram service to the Formatted Services.
    ///     This should add a new entry in the destinations dictionary
    ///     and should contain a set of length 1.
    /// </summary>
    [Test]
    public void TestAddTramService()
    {
        _formattedServices?.AddService(_tram);
        Dictionary<string, SortedSet<Tram?>?>? result = _formattedServices?.Destinations;
        Assert.NotNull(result);
        Debug.Assert(_destination != null, nameof(_destination) + " != null");
        Assert.IsTrue(result?.ContainsKey(_destination));
        var trams = result?[_destination];
        Assert.IsNotNull(trams);
        Assert.IsTrue(trams?.Count == 1);
        Assert.IsTrue(trams?.Contains(_tram));
    }

    /// <summary>
    ///     Test to add a tram with a different destination.
    ///     This should mean the dict still contains 1 item, but contains
    ///     the different key.
    /// </summary>
    [Test]
    public void TestAddDifferentDestination()
    {
        _formattedServices?.AddService(_tramDiffDestination);
        Dictionary<string, SortedSet<Tram?>?>? result = _formattedServices?.Destinations;
        Assert.NotNull(result);
        Debug.Assert(_diffDestination != null, nameof(_diffDestination) + " != null");
        Assert.IsTrue(result?.ContainsKey(_diffDestination));
        var trams = result?[_diffDestination];
        Assert.IsNotNull(trams);
        Assert.IsTrue(trams?.Count == 1);
        Assert.IsTrue(trams?.Contains(_tramDiffDestination));
    }

    /// <summary>
    ///     Test to add two trams with the same destination.
    ///     This should return a sorted set with two items.
    /// </summary>
    [Test]
    public void TestAddTramSameDestination()
    {
        _formattedServices?.AddService(_tram);
        _formattedServices?.AddService(_tramSameDestinationDiffWait);
        Dictionary<string, SortedSet<Tram?>?>? result = _formattedServices?.Destinations;
        Assert.NotNull(result);
        Debug.Assert(_destination != null, nameof(_destination) + " != null");
        Assert.IsTrue(result?.ContainsKey(_destination));
        var trams = result?[_destination];
        Assert.AreEqual(2, trams?.Count);
        Assert.IsTrue(trams?.Contains(_tram));
        Assert.IsTrue(trams?.Contains(_tramSameDestinationDiffWait));
    }

    /// <summary>
    ///     Test to validate the trams in FormattedServices are in the correct order.
    ///     They should have the lowest wait first.
    /// </summary>
    [Test]
    public void TestValidateTramsOrder()
    {
        _formattedServices?.AddService(_tram);
        _formattedServices?.AddService(_tramSameDestinationDiffWait);
        Dictionary<string, SortedSet<Tram?>?>? result = _formattedServices?.Destinations;
        Assert.NotNull(result);
        Debug.Assert(_destination != null, nameof(_destination) + " != null");
        Assert.IsTrue(result?.ContainsKey(_destination));
        var trams = result?[_destination];
        var first = trams?.First();
        Assert.AreEqual("Double", first?.Carriages);
        trams?.Remove(trams.First());
        var second = trams?.First();
        Debug.Assert(first?.Wait != null, "first?.Wait != null");
        var firstWait = int.Parse(first.Wait);
        Debug.Assert(second != null, nameof(second) + " != null");
        var secondWait = int.Parse(second.Wait);
        Assert.IsTrue(firstWait < secondWait);
    }

    /// <summary>
    /// Test to add multiple trams to diff destinations.
    /// The destination with the first tram should be displayed first
    /// </summary>
    [Test]
    public void TestValidateDiffDestinationsDiffWait()
    {
        _formattedServices?.AddService(_tram);
        _formattedServices?.AddService(_tramDiffDestinationDiffWait);
        Dictionary<string, SortedSet<Tram?>?>? result = _formattedServices?.Destinations;
        Assert.NotNull(result);
        Debug.Assert(_destination != null, nameof(_destination) + " != null");
        Assert.IsTrue(result?.ContainsKey(_destination));
        Debug.Assert(_diffDestination != null, nameof(_diffDestination) + " != null");
        Assert.IsTrue(result?.ContainsKey(_diffDestination));
        var firstDestination = result?.Keys.First();
        Assert.AreEqual(firstDestination, _diffDestination);
    }

    /// <summary>
    ///     Test to add a null tram.
    ///     This should not be added and the dict should remain empty.
    /// </summary>
    [Test]
    public void TestAddNullTram()
    {
        _formattedServices?.AddService(null);
        Dictionary<string, SortedSet<Tram?>?>? result = _formattedServices?.Destinations;
        Assert.NotNull(result);
        Assert.AreEqual(0, result?.Keys.Count);
    }

    /// <summary>
    ///     Test to add a message to the formatted services.
    ///     This should leave a set with size 1.
    /// </summary>
    [Test]
    public void TestAddMessage()
    {
        _formattedServices?.AddMessage("Example");
        Assert.NotNull(_formattedServices?.Messages);
        Assert.AreEqual(1, _formattedServices?.Messages.Count);
        Assert.IsTrue(_formattedServices?.Messages.Contains("Example"));
    }

    /// <summary>
    ///     Test to add a null message to formatted services.
    ///     This should not be added to the set, and the size should remain 0.
    /// </summary>
    [Test]
    public void TestAddNullMessage()
    {
        _formattedServices?.AddMessage(null);
        Assert.NotNull(_formattedServices?.Messages);
        Assert.AreEqual(0, _formattedServices?.Messages.Count);
    }

    /// <summary>
    /// Test to check that last updated is set.
    /// This should be set to match the value given.
    /// </summary>
    [Test]
    public void TestSetLastUpdated()
    {
        _formattedServices?.SetLastUpdated(ExampleLastUpdated);
        Assert.NotNull(_formattedServices?.LastUpdated);
        Assert.IsNotEmpty(_formattedServices?.LastUpdated);
        Assert.AreEqual(ExampleLastUpdated, _formattedServices?.LastUpdated);
    }

    /// <summary>
    /// Test to check that last updated is not set when null is passed.
    /// </summary>
    [Test]
    public void TestSetLastUpdatedNull()
    {
        _formattedServices?.SetLastUpdated(ExampleLastUpdated);
        _formattedServices?.SetLastUpdated(null);
        Assert.NotNull(_formattedServices?.LastUpdated);
        Assert.AreEqual(ExampleLastUpdated, _formattedServices?.LastUpdated);
    }

    [Test]
    public void TestSetLastUpdatedEmpty()
    {
        _formattedServices?.SetLastUpdated(ExampleLastUpdated);
        _formattedServices?.SetLastUpdated("");
        Assert.NotNull(_formattedServices?.LastUpdated);
        Assert.IsNotEmpty(_formattedServices?.LastUpdated);
    }

    [Test]
    public void TestDoNotUpdateLastUpdatedWhenSet()
    {
        _formattedServices?.SetLastUpdated(ExampleLastUpdated);
        _formattedServices?.SetLastUpdated(DiffExampleLastUpdated);
        Assert.NotNull(_formattedServices?.LastUpdated);
        Assert.IsNotEmpty(_formattedServices?.LastUpdated);
        Assert.AreEqual(ExampleLastUpdated, _formattedServices?.LastUpdated);
    }
}