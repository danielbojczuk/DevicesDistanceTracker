using Xunit;

using DistanceTrackerFunction.Domain.Devices;
using Amazon.DynamoDBv2.Model;
using DistanceTrackerFunction.Infrastructure.Repositories;
using Moq;
using Amazon.DynamoDBv2;

namespace DistanceTrackerFunction.Tests.Infrastructure.Repositories;

public class DeviceRepositoryTest
{

  [Fact]
  public async void TestGetDevice()
  {
    var itemResponse = new Dictionary<string, AttributeValue>();
    itemResponse.Add("macAddress", new AttributeValue()
    {
      S = "HH:AA:AA:AA:01",
    });
    itemResponse.Add("latitude", new AttributeValue()
    {
      N = "10",
    });
    itemResponse.Add("longitude", new AttributeValue()
    {
      N = "10",
    });
    itemResponse.Add("timestamp", new AttributeValue()
    {
      S = "1985-10-03 08:30:00",
    });
    itemResponse.Add("type", new AttributeValue()
    {
      S = "handheld",
    });
    itemResponse.Add("vehicleId", new AttributeValue()
    {
      S = "HH:AA:AA:AA:01",
    });

    var dynamoResponse = new GetItemResponse()
    {
      Item = itemResponse
    };

    var mock = new Mock<IAmazonDynamoDB>();
    mock.Setup(_ => _.GetItemAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, AttributeValue>>(), default)).ReturnsAsync(dynamoResponse);

    var repo = new DeviceRepository(mock.Object, "DevicePosition");
    var device = await repo.GetByMacAddress("VV:AA:AA:AA:01");


    var expectedDevice = new Device
    {
      DeviceType = DeviceTypeEnum.Handheld,
      LastUpdated = DateTime.Parse("1985-10-03 08:30:00"),
      Latitude = 10,
      Longitude = 10,
      MacAddress = "HH:AA:AA:AA:01"
    };
    Assert.Equal(expectedDevice, device);
  }

  [Fact]
  public async void TestGetNoHandheldMacAddress()
  {
    var itemResponse = new Dictionary<string, AttributeValue>();
    var dynamoResponse = new GetItemResponse()
    {
      Item = itemResponse
    };

    var mock = new Mock<IAmazonDynamoDB>();
    mock.Setup(_ => _.GetItemAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, AttributeValue>>(), default)).ReturnsAsync(dynamoResponse);

    var repo = new DeviceRepository(mock.Object, "DevicePosition");
    var device = await repo.GetByMacAddress("VV:AA:AA:AA:01");

    Assert.Null(device);
  }
}
