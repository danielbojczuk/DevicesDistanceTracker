using DistanceTrackerFunction.Domain.Devices;

namespace DistanceTrackerFunction.Domain.Tracker;

public interface INotificationRepository
{
  public Task SendNotification(Notification notification);
}