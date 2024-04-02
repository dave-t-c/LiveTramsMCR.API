using Amazon.DynamoDBv2.Model;

namespace LiveTramsMCR.Common.Data.DynamoDb;

public interface IDynamoDbTable
{
    public CreateTableRequest BuildCreateTableRequest();
}