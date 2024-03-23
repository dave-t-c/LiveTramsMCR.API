using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync.Helpers;
using LiveTramsMCR.DataSync.SynchronizationTasks;
using LiveTramsMCR.Models.V1.Stops;
using LiveTramsMCR.Models.V1.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using MongoDB.Driver;
using NUnit.Framework;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace LiveTramsMCR.Tests.TestDataSync;

public class TestStopSynchronization : BaseNunitTest
{
    private StopSynchronization? _stopSynchronization;
    private MongoClient? _mongoClient;
    private IStopsRepository? _stopsRepository;
    private List<Stop>? _stops;
    
    [SetUp]
    public void SetUp()
    {
        _stopSynchronization = new StopSynchronization();
        _mongoClient = TestHelper.GetService<MongoClient>();
        _stopsRepository = TestHelper.GetService<IStopsRepository>();
        var stopsPath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.StopsPath);
        _stops = FileHelper.ImportFromJsonFile<List<Stop>>(stopsPath);

    }

    [TearDown]
    public void TearDown()
    {
        _stopSynchronization = null;
        _mongoClient = null;
        _stopsRepository = null;
        _stops = null;
    }

    /// <summary>
    /// Test to create stops from an empty DB.
    /// All 99 stops should be created.
    /// </summary>
    [Test]
    public async Task TestCreateStopsFromEmptyDb()
    {
        await _stopSynchronization.SyncData(_mongoClient, _stops);

        var createdStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, createdStops.Count);
    }

    /// <summary>
    /// Test to update an existing stop wth a new value.
    /// </summary>
    [Test]
    public async Task TestUpdateExistingStop()
    {
        var db = _mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        var stopsCollection = db.GetCollection<Stop>(AppConfiguration.StopsCollectionName);
        await stopsCollection.InsertManyAsync(_stops);

        var altrinchamStop = _stops.First(stop => stop.Tlaref == "ALT");

        altrinchamStop.StopName = "Updated stop name";
        var altrinchamIndex = _stops.FindIndex(stop => stop.Tlaref == "ALT");
        _stops[altrinchamIndex] = altrinchamStop;

        await _stopSynchronization.SyncData(_mongoClient, _stops);
        
        var updatedStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, updatedStops.Count);

        var updatedAltrinchamStop = updatedStops.First(stop => stop.Tlaref == "ALT");
        Assert.AreEqual("Updated stop name", updatedAltrinchamStop.StopName);
    }

    [Test]
    public void TestDeleteExistingStop()
    {
        Assert.Fail();
    }
}