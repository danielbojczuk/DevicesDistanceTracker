using Xunit;
using DistanceTrackerFunction.Domain.Tracker;
using DistanceTrackerFunction.Domain;
using DistanceTrackerFunction.Domain.Devices;
using Moq;
using Amazon.Lambda.DynamoDBEvents;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;

namespace DistanceTrackerFunction.Tests.Domain.Tracker;

public class DistanceTrackerTest
{

  [Fact]
  public async void TestNoPairFound()
  {
    var mockPairRepo = new Mock<IDevicePairsRepository>();
    var mockDeviceRepo = new Mock<IDeviceRepository>();
    var mockNotificationRepo = new Mock<INotificationRepository>();
    var mockLogger = new Mock<ILogger>();
    mockPairRepo.Setup(_ => _.GetHandheldMacAddressByVehicle(It.IsAny<Device>())).ReturnsAsync(null as string);

    var tracker = new DistanceTracker(mockPairRepo.Object, mockDeviceRepo.Object, mockNotificationRepo.Object, mockLogger.Object);
    await tracker.Notify(this.buildDynamoDbEvent());

    mockPairRepo.Verify(t => t.GetHandheldMacAddressByVehicle(It.IsAny<Device>()), Times.Once);
    mockDeviceRepo.Verify(t => t.GetByMacAddress(It.IsAny<string>()), Times.Never);
  }

  [Fact]
  public async void TestNoHandheldFound()
  {
    var mockPairRepo = new Mock<IDevicePairsRepository>();
    var mockDeviceRepo = new Mock<IDeviceRepository>();
    var mockNotificationRepo = new Mock<INotificationRepository>();
    var mockLogger = new Mock<ILogger>();
    mockPairRepo.Setup(_ => _.GetHandheldMacAddressByVehicle(It.IsAny<Device>())).ReturnsAsync("mac2");
    mockDeviceRepo.Setup(_ => _.GetByMacAddress(It.IsAny<string>())).ReturnsAsync(null as Device?);


    var tracker = new DistanceTracker(mockPairRepo.Object, mockDeviceRepo.Object, mockNotificationRepo.Object, mockLogger.Object);
    await tracker.Notify(this.buildDynamoDbEvent());

    mockPairRepo.Verify(t => t.GetHandheldMacAddressByVehicle(It.IsAny<Device>()), Times.Exactly(1));
    mockDeviceRepo.Verify(t => t.GetByMacAddress("mac2"), Times.Exactly(1));
    mockLogger.Verify(t => t.Warning("No handheld found for the vehicle", It.IsAny<Device>()), Times.Exactly(1));
  }

  [Fact]
  public async void DistanceLessThan50()
  {
    var handheld = new Device()
    {
      DeviceType = DeviceTypeEnum.Handheld,
      LastUpdated = new DateTime(),
      Latitude = 53.32055555555557,
      Longitude = -1.7297222222222222,
      MacAddress = "mac2"
    };

    var mockPairRepo = new Mock<IDevicePairsRepository>();
    var mockDeviceRepo = new Mock<IDeviceRepository>();
    var mockNotificationRepo = new Mock<INotificationRepository>();
    var mockLogger = new Mock<ILogger>();
    mockPairRepo.Setup(_ => _.GetHandheldMacAddressByVehicle(It.IsAny<Device>())).ReturnsAsync(handheld.MacAddress);
    mockDeviceRepo.Setup(_ => _.GetByMacAddress(It.IsAny<string>())).ReturnsAsync(handheld);


    var tracker = new DistanceTracker(mockPairRepo.Object, mockDeviceRepo.Object, mockNotificationRepo.Object, mockLogger.Object);
    await tracker.Notify(this.buildDynamoDbEvent());

    mockPairRepo.Verify(t => t.GetHandheldMacAddressByVehicle(It.IsAny<Device>()), Times.Exactly(1));
    mockDeviceRepo.Verify(t => t.GetByMacAddress("mac2"), Times.Exactly(1));
    mockLogger.Verify(t => t.Warning(It.IsAny<string>(), It.IsAny<Device>()), Times.Never);
    mockNotificationRepo.Verify(t => t.SendNotification(It.IsAny<Notification>()), Times.Never);
  }

  [Fact]
  public async void DistanceOverThan50()
  {
    var handheld = new Device()
    {
      DeviceType = DeviceTypeEnum.Handheld,
      LastUpdated = new DateTime(),
      Latitude = 52.32055555555557,
      Longitude = -1.7297222222222222,
      MacAddress = "mac2"
    };

    var mockPairRepo = new Mock<IDevicePairsRepository>();
    var mockDeviceRepo = new Mock<IDeviceRepository>();
    var mockNotificationRepo = new Mock<INotificationRepository>();
    var mockLogger = new Mock<ILogger>();
    mockPairRepo.Setup(_ => _.GetHandheldMacAddressByVehicle(It.IsAny<Device>())).ReturnsAsync(handheld.MacAddress);
    mockDeviceRepo.Setup(_ => _.GetByMacAddress(It.IsAny<string>())).ReturnsAsync(handheld);

    var tracker = new DistanceTracker(mockPairRepo.Object, mockDeviceRepo.Object, mockNotificationRepo.Object, mockLogger.Object);
    await tracker.Notify(this.buildDynamoDbEvent());

    mockPairRepo.Verify(t => t.GetHandheldMacAddressByVehicle(It.IsAny<Device>()), Times.Exactly(1));
    mockDeviceRepo.Verify(t => t.GetByMacAddress("mac2"), Times.Exactly(1));
    mockLogger.Verify(t => t.Warning(It.IsAny<string>(), It.IsAny<Device>()), Times.Never);
    mockNotificationRepo.Verify(t => t.SendNotification(It.IsAny<Notification>()), Times.Exactly(1));
  }

  private DynamoDBEvent buildDynamoDbEvent()
  {
    return new DynamoDBEvent
    {
      Records = new List<DynamodbStreamRecord>()
        {
            new DynamodbStreamRecord() {
                Dynamodb = new StreamRecord() {
                    NewImage = this.BuildDynamoDbImage("macAddress",53.32055555555556,-1.7297222222222221,"vehicle","2022-10-10T16:45:33Z"),
                    OldImage = this.BuildDynamoDbImage("macAddress",53.32055555555556,-1.7297222222222221,"vehicle","2022-10-10T16:45:33Z"),
                },
                EventName = new OperationType("MODIFY")
            },
            new DynamodbStreamRecord() {
                Dynamodb = new StreamRecord() {
                    NewImage = this.BuildDynamoDbImage("macAddress",10,10,"vehicle","2022-10-10T16:45:33Z"),
                    OldImage = this.BuildDynamoDbImage("macAddress",20,20,"vehicle","2022-10-10T16:45:33Z"),
                },
                EventName = new OperationType("MODIFY")
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
