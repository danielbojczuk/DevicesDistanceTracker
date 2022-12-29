using Xunit;
using DistanceTrackerFunction.Domain.Tracker;
using DistanceTrackerFunction.Domain;
using DistanceTrackerFunction.Domain.Devices;
using Moq;

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

    var vehicle = new Device()
    {
      DeviceType = DeviceTypeEnum.Vehicle,
      LastUpdated = new DateTime(),
      Latitude = 53.32055555555556,
      Longitude = -1.7297222222222221,
      MacAddress = "mac1"
    };

    var vehicles = new List<Device>()
    {
      vehicle
    };

    var tracker = new DistanceTracker(mockPairRepo.Object, mockDeviceRepo.Object, mockNotificationRepo.Object, mockLogger.Object);
    await tracker.Notify(vehicles);

    mockPairRepo.Verify(t => t.GetHandheldMacAddressByVehicle(vehicle), Times.Exactly(1));
    mockDeviceRepo.Verify(t => t.GetByMacAddress(vehicle.MacAddress), Times.Never);
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

    var vehicle = new Device()
    {
      DeviceType = DeviceTypeEnum.Vehicle,
      LastUpdated = new DateTime(),
      Latitude = 53.32055555555556,
      Longitude = -1.7297222222222221,
      MacAddress = "mac1"
    };

    var vehicles = new List<Device>()
    {
      vehicle
    };

    var tracker = new DistanceTracker(mockPairRepo.Object, mockDeviceRepo.Object, mockNotificationRepo.Object, mockLogger.Object);
    await tracker.Notify(vehicles);

    mockPairRepo.Verify(t => t.GetHandheldMacAddressByVehicle(vehicle), Times.Exactly(1));
    mockDeviceRepo.Verify(t => t.GetByMacAddress("mac2"), Times.Exactly(1));
    mockLogger.Verify(t => t.Warning("No handheld found for the vehicle", vehicle), Times.Exactly(1));
  }

  [Fact]
  public async void DistanceLessThan50()
  {
    var vehicle = new Device()
    {
      DeviceType = DeviceTypeEnum.Vehicle,
      LastUpdated = new DateTime(),
      Latitude = 53.32055555555556,
      Longitude = -1.7297222222222221,
      MacAddress = "mac1"
    };

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



    var vehicles = new List<Device>()
    {
      vehicle
    };

    var tracker = new DistanceTracker(mockPairRepo.Object, mockDeviceRepo.Object, mockNotificationRepo.Object, mockLogger.Object);
    await tracker.Notify(vehicles);

    mockPairRepo.Verify(t => t.GetHandheldMacAddressByVehicle(vehicle), Times.Exactly(1));
    mockDeviceRepo.Verify(t => t.GetByMacAddress("mac2"), Times.Exactly(1));
    mockLogger.Verify(t => t.Warning(It.IsAny<string>(), It.IsAny<Device>()), Times.Never);
    mockNotificationRepo.Verify(t => t.SendNotification(It.IsAny<Notification>()), Times.Never);
  }

  [Fact]
  public async void DistanceOverThan50()
  {
    var vehicle = new Device()
    {
      DeviceType = DeviceTypeEnum.Vehicle,
      LastUpdated = new DateTime(),
      Latitude = 53.32055555555556,
      Longitude = -1.7297222222222221,
      MacAddress = "mac1"
    };

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



    var vehicles = new List<Device>()
    {
      vehicle
    };

    var tracker = new DistanceTracker(mockPairRepo.Object, mockDeviceRepo.Object, mockNotificationRepo.Object, mockLogger.Object);
    await tracker.Notify(vehicles);

    mockPairRepo.Verify(t => t.GetHandheldMacAddressByVehicle(vehicle), Times.Exactly(1));
    mockDeviceRepo.Verify(t => t.GetByMacAddress("mac2"), Times.Exactly(1));
    mockLogger.Verify(t => t.Warning(It.IsAny<string>(), It.IsAny<Device>()), Times.Never);
    mockNotificationRepo.Verify(t => t.SendNotification(It.IsAny<Notification>()), Times.Exactly(1));
  }
}
