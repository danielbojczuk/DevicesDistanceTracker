using DistanceTrackerFunction.Domain.Devices;

namespace DistanceTrackerFunction.Domain.Tracker;

public interface IDeviceRepository
{
  public Task<Device?> GetByMacAddress(string macAddress);
}