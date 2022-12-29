using Xunit;

using DistanceTrackerFunction.Domain.Devices;
using Amazon.DynamoDBv2.Model;
using DistanceTrackerFunction.Infrastructure.Repositories;
using Moq;
using Amazon.DynamoDBv2;

namespace DistanceTrackerFunction.Tests.Infrastructure.Repositories;

public class DevicePairsRepositoryTest
{

  [Fact]
  public async void TestGetHandheldMacAddress()
  {
    var itemResponse = new Dictionary<string, AttributeValue>();
    itemResponse.Add("vehicleMacAddress", new AttributeValue()
    {
      S = "VV:AA:AA:AA:01",
    });
    itemResponse.Add("handheldMacAddress", new AttributeValue()
    {
      S = "HH:AA:AA:AA:01",
    });
    var dynamoResponse = new GetItemResponse()
    {
      Item = itemResponse
    };
    var mock = new Mock<IAmazonDynamoDB>();
    mock.Setup(_ => _.GetItemAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, AttributeValue>>(), default)).ReturnsAsync(dynamoResponse);


    var vehicle = new Device
    {
      DeviceType = DeviceTypeEnum.Vehicle,
      LastUpdated = new DateTime(),
      Latitude = 10,
      Longitude = 10,
      MacAddress = "VV:AA:AA:AA:01"
    };
    var repo = new DevicePairsRepository(mock.Object, "DevicePairs");
    var handheldMacAddress = await repo.GetHandheldMacAddressByVehicle(vehicle);

    Assert.Equal("HH:AA:AA:AA:01", handheldMacAddress);
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

    var vehicle = new Device
    {
      DeviceType = DeviceTypeEnum.Vehicle,
      LastUpdated = new DateTime(),
      Latitude = 10,
      Longitude = 10,
      MacAddress = "VV:AA:AA:AA:01"
    };

    var repo = new DevicePairsRepository(mock.Object, "DevicePairs");
    var handheldMacAddress = await repo.GetHandheldMacAddressByVehicle(vehicle);

    Assert.Null(handheldMacAddress);
  }
}
