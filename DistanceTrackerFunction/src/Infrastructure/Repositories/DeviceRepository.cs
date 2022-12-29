using DistanceTrackerFunction.Domain.Devices;
using DistanceTrackerFunction.Domain.Tracker;
using Amazon.DynamoDBv2;

namespace DistanceTrackerFunction.Infrastructure.Repositories;

public class DeviceRepository : AbstractDynamoDbRepository, IDeviceRepository
{
  public DeviceRepository(IAmazonDynamoDB client, string tableName) : base(client, tableName) { }

  public async Task<Device?> GetByMacAddress(string macAddress)
  {
    var item = await this.GetByHashKey("macAddress", macAddress);

    if (!item.ContainsKey("macAddress"))
    {
      return null;
    }

    return new Device()
    {
      MacAddress = item["macAddress"].S,
      Latitude = Convert.ToDouble(item["latitude"].N),
      Longitude = Convert.ToDouble(item["longitude"].N),
      LastUpdated = DateTime.Parse(item["timestamp"].S),
      DeviceType = (item["type"].S == "vehicle") ? DeviceTypeEnum.Vehicle : DeviceTypeEnum.Handheld
    };
  }
}