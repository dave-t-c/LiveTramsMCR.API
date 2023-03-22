using System.Collections.Generic;
using System.Linq;
using LiveTramsMCR.Models.V1.Services;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

public class MockServiceRequester : IRequester
{
    private const string ValidApiResponsePath = "../../../Resources/ExampleApiResponse.json";

    private List<int> _expectedIds;
    public MockServiceRequester(List<int> expectedIds)
    {
        this._expectedIds = expectedIds;
    }

    public List<UnformattedServices> RequestServices(List<int> ids)
    {
        if (ids.Any(id => _expectedIds.Any(exId => exId == id)))
            return new List<UnformattedServices>
            {
                ImportServicesResponse.ImportUnformattedServices(ValidApiResponsePath) ?? new UnformattedServices()
            };

        return new List<UnformattedServices>();
    }
}