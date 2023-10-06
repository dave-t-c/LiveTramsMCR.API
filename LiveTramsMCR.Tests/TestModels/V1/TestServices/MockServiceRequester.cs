using System.Collections.Generic;
using System.Net.Http;
using LiveTramsMCR.Models.V1.Services;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

public class MockServiceRequester : IRequester
{
    private readonly HttpResponseMessage _httpResponseMessage;
    private readonly HttpResponseMessage? _httpResponseMessageAllServices;

    public MockServiceRequester(HttpResponseMessage httpResponseMessage, HttpResponseMessage? httpResponseMessageAllServices = null)
    {
        _httpResponseMessage = httpResponseMessage;
        _httpResponseMessageAllServices = httpResponseMessageAllServices;
    }

    public HttpResponseMessage RequestServices(string tlaref)
    {
        return _httpResponseMessage;
    }
    
    public HttpResponseMessage RequestServices(IEnumerable<string> tlarefs)
    {
        return _httpResponseMessage;
    }
    public HttpResponseMessage RequestAllServices()
    {
        return _httpResponseMessageAllServices!;
    }
}