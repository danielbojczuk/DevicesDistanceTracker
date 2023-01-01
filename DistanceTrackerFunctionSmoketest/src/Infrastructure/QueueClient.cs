using Amazon.SQS;
using Amazon.SQS.Model;
using DevicesDistanceTrackerSmoketest.Domain;

namespace DevicesDistanceTrackerSmoketest.Infrastructure;

public class QueueClient : IQueueClient
{
  private IAmazonSQS client { get; init; }
  private string queueUrl { get; init; }
  public QueueClient(IAmazonSQS client, string queueUrl)
  {
    this.client = client;
    this.queueUrl = queueUrl;
  }
  public async Task<List<DevicesDistanceTrackerSmoketest.Domain.Message>> ReceiveMessages()
  {
    var sqsResponse = await client.ReceiveMessageAsync(new ReceiveMessageRequest
    {
      QueueUrl = this.queueUrl,
      WaitTimeSeconds = 20
    });

    var messagesList = new List<DevicesDistanceTrackerSmoketest.Domain.Message>();

    foreach (var message in sqsResponse.Messages)
    {
      messagesList.Add(new Domain.Message()
      {
        Body = message.Body,
        ReceiptHandle = message.ReceiptHandle,
      });
    }

    return messagesList;
  }
  public async Task DeleteMessage(string ReceiptHandle)
  {
    await client.DeleteMessageAsync(this.queueUrl, ReceiptHandle);
  }
}