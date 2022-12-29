using Xunit;

using DistanceTrackerFunction.Domain.Devices;
using DistanceTrackerFunction.Infrastructure.Repositories;
using Moq;
using Amazon.SimpleNotificationService;

namespace DistanceTrackerFunction.Tests.Infrastructure.Repositories;

public class NotificationRepositoryTest
{

  [Fact]
  public async void TestGeDevice()
  {
    var expectedDevice = new Device
    {
      DeviceType = DeviceTypeEnum.Handheld,
      LastUpdated = DateTime.Parse("1985-10-03 08:30:00"),
      Latitude = 10,
      Longitude = 10,
      MacAddress = "HH:AA:AA:AA:01"
    };

    var mockSNS = new Mock<IAmazonSimpleNotificationService>();

    var repo = new NotificationRepository(mockSNS.Object, "topic");
    await repo.SendNotification(default);

    mockSNS.Verify(_ => _.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), default), Times.Once);
  }
}
