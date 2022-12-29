using Xunit;
using DistanceTrackerFunction.Domain.Devices;
using DistanceTrackerFunction.Domain.Tracker;

namespace DistanceTrackerFunction.Tests.Domain.Tracker;

public class DistanceCalculatorTest
{

  [Fact]
  public void TestDistanceCalculator()
  {
    var device1 = new Device
    {
      DeviceType = DeviceTypeEnum.Handheld,
      LastUpdated = new DateTime(),
      Latitude = 53.32055555555556,
      Longitude = -1.7297222222222221,
      MacAddress = "mac1"
    };

    var device2 = new Device
    {
      DeviceType = DeviceTypeEnum.Handheld,
      LastUpdated = new DateTime(),
      Latitude = 53.31861111111111,
      Longitude = -1.6997222222222223,
      MacAddress = "mac2"
    };

    var distance = DistanceCalculator.CalculateDistanceBetweenDevicesInMeters(device1, device2);
    Assert.Equal(2004.367836833705, distance);
  }

}
