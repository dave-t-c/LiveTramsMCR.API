using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync.Helpers;
using LiveTramsMCR.DataSync.SynchronizationTasks;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using MongoDB.Driver;
using NUnit.Framework;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace LiveTramsMCR.Tests.TestDataSync;

public class TestStopSynchronization : BaseNunitTest
{
    private StopSynchronization? _stopSynchronization;
    private MongoClient? _mongoClient;
    
    [SetUp]
    public void SetUp()
    {
        _stopSynchronization = new StopSynchronization();
        _mongoClient = TestHelper.GetService<MongoClient>();
    }

    [TearDown]
    public void TearDown()
    {
        _stopSynchronization = null;
    }

    /// <summary>
    /// Test to create stops from an empty DB.
    /// All 99 stops should be created.
    /// </summary>
    [Test]
    public async Task TestCreateStopsFromEmptyDb()
    {
        var stopsPath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.StopsPath);
        var importedStops = FileHelper.ImportFromJsonFile<List<Stop>>(stopsPath);

        await _stopSynchronization.SyncData(_mongoClient, importedStops);
    }

    /// <summary>
    /// 
    /// </summary>
    [Test]
    public void TestUpdateExistingStop()
    {
        Assert.Fail();
    }

    [Test]
    public void TestDeleteExistingStop()
    {
        Assert.Fail();
    }
}