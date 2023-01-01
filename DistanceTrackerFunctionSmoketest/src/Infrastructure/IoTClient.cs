using DevicesDistanceTrackerSmoketest.Domain;
using Amazon.IotData;
using Amazon.IotData.Model;
using System.Text;

namespace DevicesDistanceTrackerSmoketest.Infrastructure;

public class IoTClient : IIoTClient
{

  private IAmazonIotData client { get; init; }

  public IoTClient(IAmazonIotData client)
  {
    this.client = client;
  }

  public async Task publishMessage(string topic, string message)
  {
    var request = new PublishRequest()
    {
      Payload = new MemoryStream(Encoding.ASCII.GetBytes(message)),
      Topic = topic
    };

    await client.PublishAsync(request);
  }
}