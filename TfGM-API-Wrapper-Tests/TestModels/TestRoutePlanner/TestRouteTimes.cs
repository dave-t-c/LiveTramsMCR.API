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
    
    [SetUp]
    public void SetUp()
    {
        
    }

    /// <summary>
    /// Test to create new route times with an example route.
    /// When this is added, we should be able to retrieve the
    /// route times for that route.
    /// </summary>
    [Test]
    public void TestAddExampleRoute()
    {
        var routeTimes = new RouteTimes();
        var routeTimesDict = new Dictionary<string, TimeSpan>
        {
            {"Example", _initialRouteTime}
        };
        routeTimes.AddRoute("Purple", routeTimesDict);
        var result = routeTimes.GetRouteTimes("Purple");
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Keys.Count);
        var resultEntry = result.First();
        Assert.NotNull(resultEntry);
        Assert.AreEqual("Purple", resultEntry.Key);
        Assert.AreEqual(routeTimesDict, resultEntry.Value);
    }
    
    /// <summary>
    /// Test to add a route with a different name to route times.
    /// This should return the route with a different name.
    /// </summary>
    [Test]
    public void TestAddDifferentRoute()
    {
        var routeTimes = new RouteTimes();
        var routeTimesDict = new Dictionary<string, TimeSpan>
        {
            {"Different Example", _initialRouteTime}
        };
        routeTimes.AddRoute("Yellow", routeTimesDict);
        var result = routeTimes.GetRouteTimes("Yellow");
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Keys.Count);
        var resultEntry = result.First();
        Assert.NotNull(resultEntry);
        Assert.AreEqual("Yellow", resultEntry.Key);
        Assert.AreEqual(routeTimesDict, resultEntry.Value);
    }
}