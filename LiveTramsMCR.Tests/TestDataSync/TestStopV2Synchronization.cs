using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using LiveTramsMCR.Configuration;
using LiveTramsMCR.DataSync.Helpers;
using LiveTramsMCR.DataSync.SynchronizationTasks;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Models.V2.Stops.Data;
using LiveTramsMCR.Tests.Common;
using LiveTramsMCR.Tests.Helpers;
using MongoDB.Driver;
using NUnit.Framework;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace LiveTramsMCR.Tests.TestDataSync;

public class TestStopV2Synchronization : BaseNunitTest
{
    private SynchronizationTask<StopV2>? _stopSynchronization;
    private MongoClient? _mongoClient;
    private IStopsRepositoryV2? _stopsRepository;
    private List<StopV2>? _stops;
    private IMongoCollection<StopV2>? _stopsCollection;
    
    [SetUp]
    public void SetUp()
    {
        _mongoClient = TestHelper.GetService<MongoClient>();
        var dynamoDbClient = TestHelper.GetService<IAmazonDynamoDB>();
        var dynamoDbContext = TestHelper.GetService<IDynamoDBContext>();
        _stopsRepository = TestHelper.GetService<IStopsRepositoryV2>();
        var stopsPath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.StopsV2Path);
        _stops = FileHelper.ImportFromJsonFile<List<StopV2>>(stopsPath);
        var db = _mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        _stopsCollection = db.GetCollection<StopV2>(AppConfiguration.StopsV2CollectionName);
        _stopSynchronization = new SynchronizationTask<StopV2>(_stopsCollection, dynamoDbClient, dynamoDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _stopSynchronization = null;
        _mongoClient = null;
        _stopsRepository = null;
        _stops = null;
    }

    [Test]
    public async Task TestCreateStopsFromEmptyDb()
    {
        await _stopSynchronization.SyncData(_stops);

        var createdStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, createdStops.Count);
    }

    [Test]
    public async Task TestUpdateExistingStop()
    {
        await _stopsCollection.InsertManyAsync(_stops);

        var altrinchamStop = _stops.First(stop => stop.Tlaref == "ALT");

        altrinchamStop.StopName = "Updated stop name";
        var altrinchamIndex = _stops.FindIndex(stop => stop.Tlaref == "ALT");
        _stops[altrinchamIndex] = altrinchamStop;

        await _stopSynchronization.SyncData(_stops);
        
        var updatedStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, updatedStops.Count);

        var updatedAltrinchamStop = updatedStops.First(stop => stop.Tlaref == "ALT");
        Assert.AreEqual("Updated stop name", updatedAltrinchamStop.StopName);
    }

    [Test]
    public async Task TestDeleteExistingStop()
    {
        await _stopsCollection.InsertManyAsync(_stops);
        
        _stops.RemoveAll(stop => stop.Tlaref == "ALT");

        await _stopSynchronization.SyncData(_stops);
        
        var updatedStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, updatedStops.Count);
        Assert.IsNull(updatedStops.FirstOrDefault(stop => stop.Tlaref == "ALT"));
    }

    [Test]
    public async Task TestCreateUpdateDelete()
    {
        await _stopsCollection.InsertManyAsync(_stops);
        
        var altrinchamStop = _stops.First(stop => stop.Tlaref == "ALT");
        altrinchamStop.StopName = "Updated stop name";
        var altrinchamIndex = _stops.FindIndex(stop => stop.Tlaref == "ALT");
        _stops[altrinchamIndex] = altrinchamStop;
        
        _stops.RemoveAll(stop => stop.Tlaref == "ABM");

        var createdStop = new StopV2()
        {
            Tlaref = "TST",
            StopName = "Test stop"
        };
        
        _stops.Add(createdStop);

        await _stopSynchronization.SyncData(_stops);
        
        var updatedStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, updatedStops.Count);

        var updatedAltrinchamStop = updatedStops.First(stop => stop.Tlaref == "ALT");
        Assert.AreEqual("Updated stop name", updatedAltrinchamStop.StopName);
        
        Assert.IsNull(updatedStops.FirstOrDefault(stop => stop.Tlaref == "ABM"));

        var createdStopResponse = updatedStops.FirstOrDefault(stop => stop.StopName == "Test stop");
        Assert.IsNotNull(createdStopResponse);
    }
}