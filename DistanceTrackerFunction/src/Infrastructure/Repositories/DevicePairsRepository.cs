using DistanceTrackerFunction.Domain.Devices;
using DistanceTrackerFunction.Domain.Tracker;
using Amazon.DynamoDBv2;

namespace DistanceTrackerFunction.Infrastructure.Repositories;

public class DevicePairsRepository : AbstractDynamoDbRepository, IDevicePairsRepository
{
  public DevicePairsRepository(IAmazonDynamoDB client, string tableName) : base(client, tableName) { }

  public async Task<string?> GetHandheldMacAddressByVehicle(Device vehicle)
  {
    var item = await this.GetByHashKey("vehicleMacAddress", vehicle.MacAddress);

    if (!item.ContainsKey("handheldMacAddress"))
    {
      return null;
    }
    return item["handheldMacAddress"].S;
  }
}