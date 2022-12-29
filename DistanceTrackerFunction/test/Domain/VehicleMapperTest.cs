using Xunit;
using Amazon.Lambda.DynamoDBEvents;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;
using Amazon.DynamoDBv2.Model;
using DistanceTrackerFunction.Domain.Devices;

namespace DistanceTrackerFunction.Tests.Domain;

public class VehicleMapperTest
{

  [Fact]
  public void TestVehicleMapping()
  {
    var dynamoEvent = this.buildDynamoDbEvent();

    var vehicles = VehicleMapper.FromDynamoDbEvents(dynamoEvent);

    var expected = new Device[]{ new Device {
        DeviceType = DeviceTypeEnum.Vehicle,
        LastUpdated = DateTime.Parse("2022-10-10T16:45:33Z"),
        Latitude = 10,
        Longitude = 10,
        MacAddress = "macAddress"
    }};
    Assert.Equal(expected, vehicles);
  }

  private DynamoDBEvent buildDynamoDbEvent()
  {
    return new DynamoDBEvent
    {
      Records = new List<DynamodbStreamRecord>()
        {
            new DynamodbStreamRecord() {
                Dynamodb = new StreamRecord() {
                    NewImage = this.BuildDynamoDbImage("macAddress",10,10,"vehicle","2022-10-10T16:45:33Z"),
                    OldImage = this.BuildDynamoDbImage("macAddress",10,10,"vehicle","2022-10-10T16:45:33Z"),
                }
            },
            new DynamodbStreamRecord() {
                Dynamodb = new StreamRecord() {
                    NewImage = this.BuildDynamoDbImage("macAddress2",10,10,"handheld","2022-10-10T16:45:33Z"),
                    OldImage = this.BuildDynamoDbImage("macAddress2",10,10,"handheld","2022-10-10T16:45:33Z"),
                }
            },
            new DynamodbStreamRecord() {
                Dynamodb = new StreamRecord() {
                    NewImage = this.BuildDynamoDbImage("macAddress",10,10,"vehicle","2022-10-10T16:45:33Z"),
                    OldImage = this.BuildDynamoDbImage("macAddress",20,20,"vehicle","2022-10-10T16:45:33Z"),
                }
            }
        },
    };
  }

  private Dictionary<string, AttributeValue> BuildDynamoDbImage(string macAddress, double latitude, double longitude, string deviceType, string timestamp)
  {
    var image = new Dictionary<string, AttributeValue>();
    image.Add("macAddress", new AttributeValue()
    {
      S = macAddress
    });
    image.Add("type", new AttributeValue()
    {
      S = deviceType
    });
    image.Add("latitude", new AttributeValue()
    {
      N = latitude.ToString()
    });
    image.Add("longitude", new AttributeValue()
    {
      N = longitude.ToString()
    });
    image.Add("timestamp", new AttributeValue()
    {
      S = timestamp.ToString()
    });
    return image;
  }
}
