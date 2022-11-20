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
        Assert.AreEqual("Example", resultEntry.Key);
        Assert.AreEqual(_initialRouteTime, resultEntry.Value);
    }
}