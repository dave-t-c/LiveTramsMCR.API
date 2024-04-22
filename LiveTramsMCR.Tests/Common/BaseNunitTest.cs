using System.Threading.Tasks;
using LiveTramsMCR.Tests.Helpers;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.Common;

public class BaseNunitTest
{
    [TearDown]
    public async Task BaseTearDown()
    {
        MongoHelper.TearDownDatabase();
        await DynamoDbTestHelper.TearDownDatabase();
    }
}