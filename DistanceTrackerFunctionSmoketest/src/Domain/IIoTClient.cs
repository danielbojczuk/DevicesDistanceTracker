namespace DevicesDistanceTrackerSmoketest.Domain;

public interface IIoTClient
{
  public Task publishMessage(string topic, string message);
}