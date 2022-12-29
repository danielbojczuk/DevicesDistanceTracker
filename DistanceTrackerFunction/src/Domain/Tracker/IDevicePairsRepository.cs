using DistanceTrackerFunction.Domain.Devices;

namespace DistanceTrackerFunction.Domain.Tracker;

public interface IDevicePairsRepository
{
  public Task<string?> GetHandheldMacAddressByVehicle(Device vehicle);
}