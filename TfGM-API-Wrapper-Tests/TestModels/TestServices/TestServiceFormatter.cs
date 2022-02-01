using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using TfGM_API_Wrapper.Models.Services;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper_Tests.TestModels.TestServices
{
    public class TestServiceFormatter
    {
        private const string ValidApiResponsePath = "../../../Resources/ExampleApiResponse.json";
        private const string ValidApiResponsePathFourServices =
            "../../../Resources/ExampleApiResponseFourServices.json";
        private const string ValidApiResponseCaretChars = "../../../Resources/ApiResponseCaretCharsMessage.json";
        private UnformattedServices? _unformattedServices;
        private UnformattedServices? _unformattedServicesFourTrams;
        private UnformattedServices? _unformattedServicesCaretCharMessage;
        private ServiceFormatter? _serviceFormatter;
        private List<UnformattedServices?>? _unformattedServicesList;
        
        [SetUp]
        public void SetUp()
        {
            _unformattedServices = ImportUnformattedServices(ValidApiResponsePath);
            _unformattedServicesFourTrams = ImportUnformattedServices(ValidApiResponsePathFourServices);
            _unformattedServicesCaretCharMessage = ImportUnformattedServices(ValidApiResponseCaretChars);
            _serviceFormatter = new ServiceFormatter();
            _unformattedServicesList = new List<UnformattedServices?>();
             
        }

        [TearDown]
        public void TearDown()
        {
            _unformattedServices = null;
            _unformattedServicesFourTrams = null;
            _unformattedServicesCaretCharMessage = null;
            _serviceFormatter = null;
            _unformattedServicesList = null;
        }

        public UnformattedServices? ImportUnformattedServices(string path)
        {
            using var reader = new StreamReader(path);
            var jsonString = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<UnformattedServices>(jsonString);
        }

        /// <summary>
        /// Test to format services into a Formatted Services object.
        /// This should return a Formatted Services object with 1 destination and 3 trams.
        /// This should also contain a message.
        /// </summary>
        [Test]
        public void TestFormatService()
        {
            _unformattedServicesList?.Add(_unformattedServices);
            Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
            FormattedServices result = _serviceFormatter.FormatServices(_unformattedServicesList);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Destinations.Count);
            SortedSet<Tram> destResult = result.Destinations["Manchester Airport"];
            Assert.NotNull(destResult);
            Assert.AreEqual(3, destResult.Count);
            Assert.AreEqual(1, result.Messages.Count);
        }
        
        /// <summary>
        /// Test to format services with four trams.
        /// This should return a single destination with four trams.
        /// </summary>
        [Test]
        public void TestFormatServicesFourTrams()
        {
            _unformattedServicesList?.Add(_unformattedServicesFourTrams);
            Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
            FormattedServices result = _serviceFormatter.FormatServices(_unformattedServicesList);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Destinations.Count);
            SortedSet<Tram> destResult = result.Destinations["Manchester Airport"];
            Assert.NotNull(destResult);
            Assert.AreEqual(4, destResult.Count);
            Assert.AreEqual(1, result.Messages.Count);
        }

        /// <summary>
        /// Test to format a null unformatted services
        /// This should throw a null argument exception.
        /// </summary>
        [Test]
        public void TestNullUnformattedServices()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>()
                    .And.Message.EqualTo("Value cannot be null. (Parameter 'unformattedServices')"),
                delegate
                {
                    _serviceFormatter?.FormatServices(null);
                });
        }

        /// <summary>
        /// Test to format services with a message that contains caret chars.
        /// These should not be in the message added to the formatted services.
        /// </summary>
        [Test]
        public void TestFormatServicesCaretMessage()
        {
            _unformattedServicesList?.Add(_unformattedServicesCaretCharMessage);
            Debug.Assert(_serviceFormatter != null, nameof(_serviceFormatter) + " != null");
            FormattedServices result = _serviceFormatter.FormatServices(_unformattedServicesList);
            Assert.NotNull(result);
            Assert.NotNull(result.Messages);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.AreEqual("Next Altrincham Departures: Altrincham (Picc Gdns) dbl 5 min Altrincham (Market St) dbl 11 min", result.Messages.First());
        }
    }
}