using Amazon.Lambda.DynamoDBEvents;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;

namespace DistanceTrackerFunction.Domain.Devices;

public static class VehicleMapper
{
  public static List<Device> FromDynamoDbEvents(DynamoDBEvent events)
  {
    var device = new List<Device>();
    foreach (var dynamoEvent in events.Records)
    {
      if (dynamoEvent.EventName.Value != "MODIFY" || VehicleMapper.IsHandheld(dynamoEvent) || VehicleMapper.PositionChanged(dynamoEvent))
      {
        continue;
      }

      device.Add(new Device
      {
        MacAddress = dynamoEvent.Dynamodb.NewImage["macAddress"].S,
        Latitude = Convert.ToDouble(dynamoEvent.Dynamodb.NewImage["latitude"].N),
        Longitude = Convert.ToDouble(dynamoEvent.Dynamodb.NewImage["longitude"].N),
        LastUpdated = DateTime.Parse(dynamoEvent.Dynamodb.NewImage["timestamp"].S),
        DeviceType = DeviceTypeEnum.Vehicle
      });
    }
    return device;
  }

  private static bool PositionChanged(DynamodbStreamRecord record)
  {
    return (record.Dynamodb.NewImage["latitude"].N != record.Dynamodb.OldImage["latitude"].N || record.Dynamodb.NewImage["longitude"].N != record.Dynamodb.OldImage["longitude"].N);
  }

  private static bool IsHandheld(DynamodbStreamRecord record)
  {
    return record.Dynamodb.NewImage["type"].S == "handheld";
  }
}
