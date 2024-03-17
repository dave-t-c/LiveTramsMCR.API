using LiveTramsMCR.Tests.Helpers;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.Common;

public class BaseNunitTest
{
    [TearDown]
    public void BaseTearDown()
    {
        MongoHelper.TearDownDatabase();
    }
}