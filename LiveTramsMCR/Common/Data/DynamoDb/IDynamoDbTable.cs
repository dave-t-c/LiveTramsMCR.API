using Amazon.DynamoDBv2.Model;

namespace LiveTramsMCR.Common.Data.DynamoDb;

/// <summary>
/// Interface required to represent a DynamoDbTable
/// </summary>
public interface IDynamoDbTable
{
    /// <summary>
    /// Builds a CreateTableRequest for creating a new
    /// instance of this DynamoDbTable
    /// </summary>
    /// <returns></returns>
    public CreateTableRequest BuildCreateTableRequest();
}