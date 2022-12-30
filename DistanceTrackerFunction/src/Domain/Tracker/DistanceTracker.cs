using System.Diagnostics;
using Amazon.Lambda.DynamoDBEvents;
using DistanceTrackerFunction.Domain.Devices;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;

namespace DistanceTrackerFunction.Domain.Tracker;

public class DistanceTracker
{
  private IDevicePairsRepository devicePairRepository { get; set; }
  private IDeviceRepository deviceRepository { get; set; }
  private ILogger logger { get; set; }
  private INotificationRepository notificationRepository;
  public DistanceTracker(IDevicePairsRepository devicePairRepo, IDeviceRepository deviceRepo, INotificationRepository notificationRepo, ILogger logger)
  {
    this.devicePairRepository = devicePairRepo;
    this.deviceRepository = deviceRepo;
    this.logger = logger;
    this.notificationRepository = notificationRepo;
  }

  public async Task Notify(DynamoDBEvent events)
  {
    foreach (var dynamoEvent in events.Records)
    {
      if (this.PositionChanged(dynamoEvent))
      {
        continue;
      }

      var vehicle = new Device
      {
        MacAddress = dynamoEvent.Dynamodb.NewImage["macAddress"].S,
        Latitude = Convert.ToDouble(dynamoEvent.Dynamodb.NewImage["latitude"].N),
        Longitude = Convert.ToDouble(dynamoEvent.Dynamodb.NewImage["longitude"].N),
        LastUpdated = DateTime.Parse(dynamoEvent.Dynamodb.NewImage["timestamp"].S),
        DeviceType = DeviceTypeEnum.Vehicle
      };

      try
      {
        await this.ProcessVehicle(vehicle);
      }
      catch (Exception ex)
      {
        this.logger.Error("Error Processing Vehicle", ex, vehicle);
      }
    }
  }

  private async Task ProcessVehicle(Device vehicle)
  {
    var handheldMacAddress = await this.devicePairRepository.GetHandheldMacAddressByVehicle(vehicle);
    if (handheldMacAddress == null)
    {
      return;
    }

    var handheld = await this.deviceRepository.GetByMacAddress((string)handheldMacAddress);

    if (handheld == null)
    {
      this.logger.Warning("No handheld found for the vehicle", vehicle);
      return;
    }

    var distance = DistanceCalculator.CalculateDistanceBetweenDevicesInMeters((Device)handheld, vehicle);

    if (distance > 50)
    {
      await this.notificationRepository.SendNotification(new Notification()
      {
        HandheldId = ((Device)handheld).MacAddress,
        VehicleId = ((Device)vehicle).MacAddress,
        Latitude = ((Device)vehicle).Latitude,
        Longitude = ((Device)vehicle).Longitude,
      });
    }
  }

  private bool PositionChanged(DynamodbStreamRecord record)
  {
    return (record.Dynamodb.NewImage["latitude"].N != record.Dynamodb.OldImage["latitude"].N || record.Dynamodb.NewImage["longitude"].N != record.Dynamodb.OldImage["longitude"].N);
  }
}