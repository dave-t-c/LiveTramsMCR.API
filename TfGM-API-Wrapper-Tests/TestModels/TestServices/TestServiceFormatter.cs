using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Services;

namespace TfGM_API_Wrapper_Tests.TestModels.TestServices;

public class TestServiceFormatter
{
    private const string ValidApiResponsePath = "../../../Resources/ExampleApiResponse.json";

    private const string ValidApiResponsePathFourServices =
        "../../../Resources/ExampleApiResponseFourServices.json";

    private const string ValidApiResponseCaretChars = "../../../Resources/ApiResponseCaretCharsMessage.json";
    private const string ValidApiResponseEmptyMessage = "../../../Resources/ApiResponseNoMessage.json";
    private const string ValidApiResponseIncorrectOrder = "../../../Resources/ApiResponseIncorrectTramOrder.json";
    private ServiceFormatter? _serviceFormatter;
    private UnformattedServices? _unformattedServices;
    private UnformattedServices? _unformattedServicesCaretCharMessage;
    private UnformattedServices? _unformattedServicesEmptyMessage;
    private UnformattedServices? _unformattedServicesFourTrams;
    private UnformattedServices? _unformattedServicesIncorrectOrder;
    private List<UnformattedServices?>? _unformattedServicesList;

    [SetUp]
    public void SetUp()
    {
        _unformattedServices = ImportServicesResponse.ImportUnformattedServices(ValidApiResponsePath);
        _unformattedServicesFourTrams = ImportServicesResponse.ImportUnformattedServices(ValidApiResponsePathFourServices);
        _unformattedServicesCaretCharMessage = ImportServicesResponse.ImportUnformattedServices(ValidApiResponseCaretChars);
        _unformattedServicesEmptyMessage = ImportServicesResponse.ImportUnformattedServices(ValidApiResponseEmptyMessage);
        _unformattedServicesIncorrectOrder =
            ImportServicesResponse.ImportUnformattedServices(ValidApiResponseIncorrectOrder);
        _serviceFormatter = new ServiceFormatter();
        _unformattedServicesList = new List<UnformattedServices?>();
    }

    [TearDown]
    public void TearDown()
    {
        _unformattedServices = null;
        _unformattedServicesFourTrams = null;
        _unformattedServicesCaretCharMessage = null;
        _unformattedServicesEmptyMessage = null;
        _unformattedServicesIncorrectOrder = null;
        _serviceFormatter = null;
        _unformattedServicesList = null;
    }

    /// <summary>
    ///     Test to format services into a Formatted Services object.
    ///     This should return a Formatted Services object with 1 destination and 3 trams.
    ///     This should also contain a message.
    /// </summary>
    [Test]
    public void TestFormatService()
    {
        _unformattedServicesList?.Add(_unformattedServices);
        Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
        var result = _serviceFormatter.FormatServices(_unformattedServicesList);
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Destinations.Count);
        SortedSet<Tram> destResult = result.Destinations["Manchester Airport"];
        Assert.NotNull(destResult);
        Assert.AreEqual(3, destResult.Count);
        Assert.AreEqual(1, result.Messages.Count);
    }

    /// <summary>
    ///     Test to format services with four trams.
    ///     This should return a single destination with four trams.
    /// </summary>
    [Test]
    public void TestFormatServicesFourTrams()
    {
        _unformattedServicesList?.Add(_unformattedServicesFourTrams);
        Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
        var result = _serviceFormatter.FormatServices(_unformattedServicesList);
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Destinations.Count);
        SortedSet<Tram> destResult = result.Destinations["Manchester Airport"];
        Assert.NotNull(destResult);
        Assert.AreEqual(4, destResult.Count);
        Assert.AreEqual(1, result.Messages.Count);
    }

    /// <summary>
    ///     Test to format a null unformatted services
    ///     This should throw a null argument exception.
    /// </summary>
    [Test]
    public void TestNullUnformattedServices()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'unformattedServices')"),
            delegate { _serviceFormatter?.FormatServices(null); });
    }

    /// <summary>
    ///     Test to format services with a message that contains caret chars.
    ///     These should not be in the message added to the formatted services.
    /// </summary>
    [Test]
    public void TestFormatServicesCaretMessage()
    {
        _unformattedServicesList?.Add(_unformattedServicesCaretCharMessage);
        Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
        var result = _serviceFormatter.FormatServices(_unformattedServicesList);
        Assert.NotNull(result);
        Assert.NotNull(result.Messages);
        Assert.AreEqual(1, result.Messages.Count);
        Assert.AreEqual(
            "Next Altrincham Departures: Altrincham (Picc Gdns) dbl 5 min Altrincham (Market St) dbl 11 min",
            result.Messages.First());
    }

    /// <summary>
    ///     Test to add a null message to a formatted service.
    ///     This should not add the message to the formatted service.
    ///     The message set should instead be the empty set.
    /// </summary>
    [Test]
    public void TestFormatServicesNullMessage()
    {
        Debug.Assert(_unformattedServices != null, nameof(_unformattedServices) + " != null");
        _unformattedServices.MessageBoard = null;
        _unformattedServicesList?.Add(_unformattedServices);
        Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
        var result = _serviceFormatter.FormatServices(_unformattedServicesList);
        Assert.NotNull(result);
        Assert.NotNull(result.Messages);
        Assert.AreEqual(0, result.Messages.Count);
    }

    /// <summary>
    ///     Test to handle an empty message
    ///     This needs to meet the empty message format given in the response
    /// </summary>
    [Test]
    public void TestFormatServicesEmptyMessage()
    {
        _unformattedServicesList?.Add(_unformattedServicesEmptyMessage);
        Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
        var result = _serviceFormatter.FormatServices(_unformattedServicesList);
        Assert.NotNull(result);
        Assert.NotNull(result.Messages);
        Assert.AreEqual(0, result.Messages.Count);
    }

    /// <summary>
    ///     Test to handle when the message is an empty string.
    ///     This should not add the message and the messages should remain empty.
    /// </summary>
    [Test]
    public void TestFormatServicesEmptyString()
    {
        Debug.Assert(_unformattedServices != null, nameof(_unformattedServices) + " != null");
        _unformattedServices.MessageBoard = "";
        _unformattedServicesList?.Add(_unformattedServices);
        Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
        var result = _serviceFormatter.FormatServices(_unformattedServicesList);
        Assert.NotNull(result);
        Assert.NotNull(result.Messages);
        Assert.AreEqual(0, result.Messages.Count);
    }

    /// <summary>
    ///     Test to combine multiple unformatted services.
    ///     The result should include all destinations, 1 message and 3 destinations.
    /// </summary>
    [Test]
    public void TestFormatMultipleServices()
    {
        _unformattedServicesList?.Add(_unformattedServices);
        _unformattedServicesList?.Add(_unformattedServicesEmptyMessage);
        Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
        var result = _serviceFormatter.FormatServices(_unformattedServicesList);
        Assert.NotNull(result);
        Assert.NotNull(result.Messages);
        Assert.AreEqual(1, result.Messages.Count);
        Assert.AreEqual(3, result.Destinations.Count);
    }

    /// <summary>
    ///     Test to ensure the returned trams are in the correct order.
    ///     This test case uses ApiResponseIncorrectTramOrder.json file as an example,
    ///     as the services shown were returned in 13 min, 25 min, then 3 mins.
    /// </summary>
    [Test]
    public void TestEnsureTramsInCorrectOrder()
    {
        _unformattedServicesList?.Add(_unformattedServicesIncorrectOrder);
        Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
        var result = _serviceFormatter.FormatServices(_unformattedServicesList);
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Destinations.Count);
        SortedSet<Tram> trams = result.Destinations["Piccadilly"];
        var firstTram = trams.First();
        Assert.AreEqual("3", firstTram.Wait);
    }

    /// <summary>
    /// Test formatting departureBoardServices with valid unformatted services.
    /// </summary>
    [Test]
    public void TestFormatDepartureBoardServices()
    {
        _unformattedServicesList?.Add(_unformattedServices);
        var result = _serviceFormatter?.FormatDepartureBoardServices(_unformattedServicesList);
        Assert.IsNotNull(result);
        var trams = result?.Trams;
        Assert.AreEqual(3, trams?.Count);
        var firstTram = trams?.First();
        Assert.AreEqual("Manchester Airport", firstTram?.Destination);
        Assert.AreEqual("0", firstTram?.Wait);
        var finalTram = trams?.Last();
        Assert.AreEqual("Manchester Airport", finalTram?.Destination);
        Assert.AreEqual("23", finalTram?.Wait);
        
        // Check there are 3 distinct trams
        Assert.AreEqual(3, trams?.Distinct().Count());
    }
}