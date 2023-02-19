using System;
using LiveTramsMCR.Models.Services;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.TestServices;

public class TestTram
{
    private string? _carriages;
    private string? _destination;
    private string? _diffCarriages;
    private string? _diffDestination;
    private string? _diffStatus;
    private Tram? _diffTram;
    private string? _diffWait;
    private Tram? _duplicateTram;
    private string? _status;

    private Tram? _tram;
    private Tram? _tramDiffCarriages;
    private Tram? _tramDiffDest;
    private Tram? _tramDiffStatus;
    private Tram? _tramDiffWait;
    private string? _wait;

    [SetUp]
    public void SetUp()
    {
        _destination = "Example Destination";
        _diffDestination = "Different Destination";
        _carriages = "Single";
        _diffCarriages = "Double";
        _status = "Due";
        _diffStatus = "Departing";
        _wait = "9";
        _diffWait = "0";

        _tram = new Tram(_destination, _carriages, _status, _wait);
        _duplicateTram = new Tram(_destination, _carriages, _status, _wait);
        _tramDiffDest = new Tram(_diffDestination, _carriages, _status, _wait);
        _tramDiffCarriages = new Tram(_destination, _diffCarriages, _status, _wait);
        _tramDiffStatus = new Tram(_destination, _carriages, _diffStatus, _wait);
        _tramDiffWait = new Tram(_destination, _carriages, _status, _diffWait);
        _diffTram = new Tram(_diffDestination, _diffCarriages, _diffStatus, _diffWait);
    }

    [TearDown]
    public void TearDown()
    {
        _destination = null;
        _diffDestination = null;
        _carriages = null;
        _diffCarriages = null;
        _status = null;
        _diffStatus = null;
        _wait = null;
        _diffWait = null;

        _tram = null;
        _duplicateTram = null;
        _tramDiffDest = null;
        _tramDiffCarriages = null;
        _tramDiffStatus = null;
        _tramDiffWait = null;
        _diffTram = null;
    }

    /// <summary>
    ///     Test to try and get a new tram and verify the set dest, carriages, status and wait
    ///     time.
    /// </summary>
    [Test]
    public void TestCreateNewTram()
    {
        Assert.NotNull(_tram);
        Assert.AreEqual(_destination, _tram?.Destination);
        Assert.AreEqual(_carriages, _tram?.Carriages);
        Assert.AreEqual(_status, _tram?.Status);
        Assert.AreEqual(_wait, _tram?.Wait);
    }

    /// <summary>
    ///     Test to create a new tram with different values.
    ///     This resulting tram should have the new values and not the values
    ///     from the previous test.
    /// </summary>
    [Test]
    public void TestCreateDifferentTram()
    {
        Assert.NotNull(_diffTram);
        Assert.AreEqual(_diffDestination, _diffTram?.Destination);
        Assert.AreEqual(_diffCarriages, _diffTram?.Carriages);
        Assert.AreEqual(_diffStatus, _diffTram?.Status);
        Assert.AreEqual(_diffWait, _diffTram?.Wait);
    }

    /// <summary>
    ///     Create a tram with a null destination
    ///     This should throw an null args exception
    /// </summary>
    [Test]
    public void TestTramNullDest()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'destination')"),
            delegate
            {
                var unused = new Tram(null, _carriages, _status, _wait);
            });
    }

    /// <summary>
    ///     Create a tram with null carriages.
    ///     This should throw a Arg null exception.
    /// </summary>
    [Test]
    public void TestTramNullCarriages()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'carriages')"),
            delegate
            {
                var unused = new Tram(_destination, null, _status, _wait);
            });
    }

    /// <summary>
    ///     Creates a tram with a null status.
    ///     This should throw an arg null exception
    /// </summary>
    [Test]
    public void TestTramNullStatus()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'status')"),
            delegate
            {
                var unused = new Tram(_destination, _carriages, null, _wait);
            });
    }


    /// <summary>
    ///     Create a tram with a null wait.
    ///     This should throw a arg null exception.
    /// </summary>
    [Test]
    public void TestTramNullWait()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'wait')"),
            delegate
            {
                var unused = new Tram(_destination, _carriages, _status, null);
            });
    }

    /// <summary>
    ///     Check if two identical trams are equal.
    ///     This should return true for both == and Equals()
    /// </summary>
    [Test]
    public void TestIdenticalTramsEqual()
    {
        Assert.True(_tram?.Equals(_duplicateTram));
    }

    /// <summary>
    ///     Test to see if trams with a different destination are equal.
    ///     This should return false
    /// </summary>
    [Test]
    public void TestDiffDestTramsEqual()
    {
        Assert.False(_tram?.Equals(_tramDiffDest));
    }

    /// <summary>
    ///     Compare trams with different carriages to see if equal.
    ///     This should return false.
    /// </summary>
    [Test]
    public void TestDiffCarriagesEqual()
    {
        Assert.False(_tram?.Equals(_tramDiffCarriages));
    }

    /// <summary>
    ///     Compare trams with different status values to see if equal.
    ///     This should return false.
    /// </summary>
    [Test]
    public void TestDiffStatusEquals()
    {
        Assert.False(_tram?.Equals(_tramDiffStatus));
    }

    /// <summary>
    ///     Compare trams with different waits to see if equal.
    ///     This should return false.
    /// </summary>
    [Test]
    public void TestDiffWaitEquals()
    {
        Assert.False(_tram?.Equals(_tramDiffWait));
    }

    /// <summary>
    ///     Test to determine if a different object type
    ///     is equal.
    ///     This should return false.
    /// </summary>
    [Test]
    public void TestStringEquals()
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.False(_tram?.Equals("Test"));
    }

    /// <summary>
    ///     Test to check if null is equal to tram.
    ///     This should return false.
    /// </summary>
    [Test]
    public void TestNullEquals()
    {
        Assert.False(_tram?.Equals(null));
    }

    /// <summary>
    ///     Test to try and get the same hash code for
    ///     trams with the same values.
    /// </summary>
    [Test]
    public void TestGetSameHashCode()
    {
        Assert.AreEqual(_tram?.GetHashCode(), _duplicateTram?.GetHashCode());
    }

    /// <summary>
    ///     Test to get a different hash code for trams with
    ///     different destinations
    /// </summary>
    [Test]
    public void TestHashCodeDiffDestination()
    {
        Assert.AreNotEqual(_tram?.GetHashCode(), _tramDiffDest?.GetHashCode());
    }

    /// <summary>
    ///     Compare hash codes for trams with different carriage values.
    ///     These should be different values.
    /// </summary>
    [Test]
    public void TestHashCodeDiffCarriages()
    {
        Assert.AreNotEqual(_tram?.GetHashCode(), _tramDiffCarriages?.GetHashCode());
    }

    /// <summary>
    ///     Compare hash codes with different status values.
    ///     This should return different values.
    /// </summary>
    [Test]
    public void TestHashCodeDifferentStatus()
    {
        Assert.AreNotEqual(_tram?.GetHashCode(), _tramDiffStatus?.GetHashCode());
    }

    /// <summary>
    ///     Test to compare the hash code values for trams with only different wait values.
    ///     This should return different values.
    /// </summary>
    [Test]
    public void TestHashCodeDifferentWaits()
    {
        Assert.AreNotEqual(_tram?.GetHashCode(), _tramDiffWait?.GetHashCode());
    }
}