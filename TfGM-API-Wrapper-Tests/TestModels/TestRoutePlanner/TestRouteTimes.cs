using System;
using System.Collections.Generic;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.RoutePlanner;
using System.Globalization;
using System.Linq;
using DateTime = System.DateTime;

namespace TfGM_API_Wrapper_Tests.TestModels.TestRoutePlanner;

public class TestRouteTimes
{
    private const string ExampleTimeString = "08:40:00";
    private readonly TimeSpan _initialRouteTime = TimeSpan.Parse(ExampleTimeString);
    private RouteTimes? _routeTimes;
    private Dictionary<string, TimeSpan>? _routeTimesDict;
    
    [SetUp]
    public void SetUp()
    {
        _routeTimes = new RouteTimes();
        _routeTimesDict = new Dictionary<string, TimeSpan>();
    }

    [TearDown]
    public void TearDown()
    {
        _routeTimes = null;
        _routeTimesDict = null;
    }

    /// <summary>
    /// Test to create new route times with an example route.
    /// When this is added, we should be able to retrieve the
    /// route times for that route.
    /// </summary>
    [Test]
    public void TestAddExampleRoute()
    {
        _routeTimesDict!["Example"] = _initialRouteTime;
        _routeTimes?.AddRoute("Purple", _routeTimesDict);
        var result = _routeTimes?.GetRouteTimes("Purple");
        Assert.NotNull(result);
        Assert.AreEqual(1, result?.Keys.Count);
        var resultEntry = result!.First();
        Assert.NotNull(resultEntry);
        var routeTimesEntry = _routeTimesDict.First();
        Assert.NotNull(routeTimesEntry);
        Assert.AreEqual(routeTimesEntry.Key, resultEntry.Key);
        Assert.AreEqual(routeTimesEntry.Value, resultEntry.Value);
    }
    
    /// <summary>
    /// Test to add a route with a different name to route times.
    /// This should return the route with a different name.
    /// </summary>
    [Test]
    public void TestAddDifferentRoute()
    {
        _routeTimesDict!["Different Example"] = _initialRouteTime;
        _routeTimes?.AddRoute("Yellow", _routeTimesDict);
        var result = _routeTimes?.GetRouteTimes("Yellow");
        Assert.NotNull(result);
        Assert.AreEqual(1, result?.Keys.Count);
        var resultEntry = result!.First();
        Assert.NotNull(resultEntry);
        var routeTimesEntry = _routeTimesDict.First();
        Assert.NotNull(routeTimesEntry);
        Assert.AreEqual(routeTimesEntry.Key, resultEntry.Key);
        Assert.AreEqual(routeTimesEntry.Value, resultEntry.Value);
    }

    /// <summary>
    /// Test to add a null route name.
    /// This should throw an illegal args exception
    /// </summary>
    [Test]
    public void TestAddNullRouteName()
    {
        _routeTimesDict!["Example"] = _initialRouteTime;
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'routeName')"),
            delegate
            { 
                _routeTimes?.AddRoute(null, _routeTimesDict);
            });
    }

    /// <summary>
    /// Test to add a null route times dict.
    /// This should thrown an args null exception.
    /// </summary>
    [Test]
    public void TestAddNullRouteTimes()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stopsDict')"),
            delegate
            { 
                _routeTimes?.AddRoute("Purple", null);
            });
    }
}