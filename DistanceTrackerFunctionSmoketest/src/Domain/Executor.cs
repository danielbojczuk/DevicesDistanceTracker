namespace DevicesDistanceTrackerSmoketest.Domain;

public class Executor
{
  private IIoTClient iotClient { get; init; }
  private IQueueClient queueClient { get; init; }
  public Executor(IIoTClient iotClient, IQueueClient queueClient)
  {
    this.iotClient = iotClient;
    this.queueClient = queueClient;
  }

  public async Task Test(string vehicleId, string expectedNotification)
  {
    await this.PublishMessage(vehicleId);
    await this.ReceiveMessages(expectedNotification);
  }

  private async Task PublishMessage(string vehicleId)
  {
    var payload = this.ReturnPublishPayload(vehicleId);
    var topic = $"v1/gps/vehicle/{vehicleId}";
    await this.iotClient.publishMessage(topic, payload);
  }

  private async Task ReceiveMessages(string expectedNotification)
  {
    var foundMessage = false;
    for (var i = 0; i < 3; i++)
    {
      Thread.Sleep(10000);

      var messages = await this.queueClient.ReceiveMessages();

      foreach (var message in messages)
      {
        if (message.Body == expectedNotification)
        {
          foundMessage = true;
          await this.queueClient.DeleteMessage(message.ReceiptHandle);
          break;
        }
      }

      if (foundMessage)
      {
        break;
      }
    }

    if (!foundMessage)
    {
      throw new Exception("Expected notification has not been received.");
    }
  }

  private string ReturnPublishPayload(string vehicleId)
  {
    return "{" +
      $"\"vehicleId\": \"{vehicleId}\"" +
      @"""latitude"": 53.236545,
      ""longitude"": 5.693435,
      ""timestamp"": """ + (DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ")) + "\"" +
    "}";
  }
}
