using DistanceTrackerFunction.Domain.Devices;

namespace DistanceTrackerFunction.Domain;

public interface ILogger
{
  public void Warning(string message, object? data);
  public void Error(string message, Exception exception);
}