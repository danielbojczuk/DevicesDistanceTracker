using Amazon.IoT.Model;

namespace DevicesDistanceTrackerSmoketest.Domain;

public interface IQueueClient
{
  public Task<List<Message>> ReceiveMessages();
  public Task DeleteMessage(string ReceiptHandle);
}