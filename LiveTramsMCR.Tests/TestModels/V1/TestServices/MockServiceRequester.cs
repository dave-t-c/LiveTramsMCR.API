using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using LiveTramsMCR.Models.V1.Services;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

public class MockServiceRequester : IRequester
{
    private readonly HttpResponseMessage? _httpResponseMessage;
    
    public MockServiceRequester(HttpResponseMessage httpResponseMessage)
    {
        _httpResponseMessage = httpResponseMessage;
    }

    public List<HttpResponseMessage> RequestServices(List<int> ids)
    {
        return new List<HttpResponseMessage>()
        {
            _httpResponseMessage!
        };
    }
}