using System;
using System.Diagnostics;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.Services;

namespace TfGM_API_Wrapper_Tests.TestModels.TestServices;

public class TestServicesDataModel
{
    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private IRequester? _requester;
    private ServicesDataModel? _servicesDataModel;

    [SetUp]
    public void SetUp()
    {
        _resourcesConfig = new ResourcesConfig
        {
            StopResourcePath = "../../../Resources/ValidStopLoader.json",
            StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json",
            TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json"
        };
        _importedResources = new ResourceLoader(_resourcesConfig).ImportResources();
        _requester = new MockServiceRequester();
        _servicesDataModel = new ServicesDataModel(_importedResources, _requester);
    }

    [TearDown]
    public void TearDown()
    {
        _resourcesConfig = null;
        _importedResources = null;
        _requester = null;
        _servicesDataModel = null;
    }

    /// <summary>
    /// Request services for 'BMR".
    /// This is one of the values returned in the mock service requester, with 1 destination expected.
    /// </summary>
    [Test]
    public void TestRequestExpectedService()
    {
        var result = _servicesDataModel?.RequestServices("BMR");
        Assert.NotNull(result);
        Debug.Assert(result != null, nameof(result) + " != null");
        Assert.AreEqual(1, result.Destinations.Count);
    }

    /// <summary>
    /// Request services for a null stop.
    /// This should throw an argument null exception.
    /// </summary>
    [Test]
    public void TestNullServiceLocationName()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stop')"),
            delegate
            {
                Debug.Assert(_servicesDataModel != null, nameof(_servicesDataModel) + " != null");
                _servicesDataModel.RequestServices(null);
            });
    }
}