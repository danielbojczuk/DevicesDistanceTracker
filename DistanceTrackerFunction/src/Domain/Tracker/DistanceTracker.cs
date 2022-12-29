using DistanceTrackerFunction.Domain.Devices;

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

  public async Task Notify(List<Device> vehicles)
  {
    foreach (var vehicle in vehicles)
    {
      var handheldMacAddress = await this.devicePairRepository.GetHandheldMacAddressByVehicle(vehicle);

      if (handheldMacAddress == null)
      {
        continue;
      }

      var handheld = await this.deviceRepository.GetByMacAddress((string)handheldMacAddress);

      if (handheld == null)
      {
        this.logger.Warning("No handheld found for the vehicle", vehicle);
        continue;
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
  }
}