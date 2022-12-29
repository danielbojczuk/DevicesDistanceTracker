using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DistanceTrackerFunction.Infrastructure.Repositories;

public abstract class AbstractDynamoDbRepository
{
  private IAmazonDynamoDB dynamoDbClient { get; init; }

  private string tableName { get; init; }

  public AbstractDynamoDbRepository(IAmazonDynamoDB client, string tableName)
  {
    this.dynamoDbClient = client;
    this.tableName = tableName;
  }

  protected async Task<Dictionary<string, AttributeValue>> GetByHashKey(string key, string value)
  {
    var hashKey = new Dictionary<string, AttributeValue>();
    hashKey.Add(key, new AttributeValue() { S = value });

    var tableReturn = await this.dynamoDbClient.GetItemAsync(this.tableName, hashKey);

    return tableReturn.Item;
  }
}