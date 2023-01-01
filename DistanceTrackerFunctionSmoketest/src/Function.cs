using Amazon.Lambda.Core;
using Amazon.IotData;
using Amazon.SQS;
using DevicesDistanceTrackerSmoketest.Domain;
using DevicesDistanceTrackerSmoketest.Infrastructure;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DevicesDistanceTrackerSmoketest;

public class Function
{
  public async Task FunctionHandler(object? input, ILambdaContext context)
  {
    var vehicleId = Environment.GetEnvironmentVariable("VEHICLE_ID");
    var expectedNotification = Environment.GetEnvironmentVariable("EXPECTED_NOTIFICATION");
    var iotEndpoint = Environment.GetEnvironmentVariable("IOT_ENDPOINT");
    var queueUrl = Environment.GetEnvironmentVariable("QUEUE_URL");

    var iotClient = new IoTClient(new AmazonIotDataClient(iotEndpoint));
    var sqsClient = new QueueClient(new AmazonSQSClient(), queueUrl);

    var executor = new Executor(iotClient, sqsClient);

    await executor.Test(vehicleId, expectedNotification);
  }
}
