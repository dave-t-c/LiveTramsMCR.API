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
    private MongoClient? _mongoClient;
    private IStopsRepository? _stopsRepository;
    private List<Stop>? _stops;
    private IMongoCollection<Stop>? _stopsCollection;
    private SynchronizationTask<Stop>? _synchronizationTask;
    
    [SetUp]
    public void SetUp()
    {
        _mongoClient = TestHelper.GetService<MongoClient>();
        var dynamoDbClient = TestHelper.GetService<IAmazonDynamoDB>();
        var dynamoDbContext = TestHelper.GetService<IDynamoDBContext>();
        _stopsRepository = TestHelper.GetService<IStopsRepository>();
        var stopsPath = Path.Combine(Environment.CurrentDirectory, AppConfiguration.StopsPath);
        _stops = FileHelper.ImportFromJsonFile<List<Stop>>(stopsPath);
        var db = _mongoClient.GetDatabase(AppConfiguration.DatabaseName);
        _stopsCollection = db.GetCollection<Stop>(AppConfiguration.StopsCollectionName);
        _synchronizationTask = new SynchronizationTask<Stop>(_stopsCollection, dynamoDbClient, dynamoDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _mongoClient = null;
        _stopsRepository = null;
        _stops = null;
        _synchronizationTask = null;
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, null);
    }

    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestCreateStopsFromEmptyDb(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _synchronizationTask.SyncData(_stops);

        var createdStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, createdStops.Count);
    }

    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestUpdateExistingStop(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _stopsCollection.InsertManyAsync(_stops);

        var altrinchamStop = _stops.First(stop => stop.Tlaref == "ALT");

        altrinchamStop.StopName = "Updated stop name";
        var altrinchamIndex = _stops.FindIndex(stop => stop.Tlaref == "ALT");
        _stops[altrinchamIndex] = altrinchamStop;

        await _synchronizationTask.SyncData(_stops);
        
        var updatedStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, updatedStops.Count);

        var updatedAltrinchamStop = updatedStops.First(stop => stop.Tlaref == "ALT");
        Assert.AreEqual("Updated stop name", updatedAltrinchamStop.StopName);
    }

    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestDeleteExistingStop(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _stopsCollection.InsertManyAsync(_stops);
        
        _stops.RemoveAll(stop => stop.Tlaref == "ALT");

        await _synchronizationTask.SyncData(_stops);
        
        var updatedStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, updatedStops.Count);
        Assert.IsNull(updatedStops.FirstOrDefault(stop => stop.Tlaref == "ALT"));
    }

    [Test]
    [TestCase("true")]
    [TestCase("false")]
    public async Task TestCreateUpdateDelete(string dynamoDbEnabled)
    {
        Environment.SetEnvironmentVariable(AppConfiguration.DynamoDbEnabledKey, dynamoDbEnabled);

        await _stopsCollection.InsertManyAsync(_stops);
        
        var altrinchamStop = _stops.First(stop => stop.Tlaref == "ALT");
        altrinchamStop.StopName = "Updated stop name";
        var altrinchamIndex = _stops.FindIndex(stop => stop.Tlaref == "ALT");
        _stops[altrinchamIndex] = altrinchamStop;
        
        _stops.RemoveAll(stop => stop.Tlaref == "ABM");

        var createdStop = new Stop()
        {
            Tlaref = "TST",
            StopName = "Test stop"
        };
        
        _stops.Add(createdStop);

        await _synchronizationTask.SyncData(_stops);
        
        var updatedStops = _stopsRepository.GetAll();
        Assert.AreEqual(_stops.Count, updatedStops.Count);

        var updatedAltrinchamStop = updatedStops.First(stop => stop.Tlaref == "ALT");
        Assert.AreEqual("Updated stop name", updatedAltrinchamStop.StopName);
        
        Assert.IsNull(updatedStops.FirstOrDefault(stop => stop.Tlaref == "ABM"));

        var createdStopResponse = updatedStops.FirstOrDefault(stop => stop.StopName == "Test stop");
        Assert.IsNotNull(createdStopResponse);
    }
}