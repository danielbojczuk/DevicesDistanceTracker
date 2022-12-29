using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.SimpleNotificationService;
using DistanceTrackerFunction.Domain.Devices;
using DistanceTrackerFunction.Domain.Tracker;
using DistanceTrackerFunction.Infrastructure.Logger;
using DistanceTrackerFunction.Infrastructure.Repositories;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DistanceTrackerFunction;

public class Function
{
  public async Task FunctionHandler(DynamoDBEvent input, ILambdaContext context)
  {
    var vehicles = VehicleMapper.FromDynamoDbEvents(input);

    var logger = new Logger();

    var dynamoDbClient = new AmazonDynamoDBClient();
    var snsClient = new AmazonSimpleNotificationServiceClient();
    var devicePairRepo = new DevicePairsRepository(dynamoDbClient, "DevicePairs");
    var deviceRepo = new DeviceRepository(dynamoDbClient, "DevicesLocation");
    var snsRepo = new NotificationRepository(snsClient, "arn:aws:sns:us-east-1:662642131450:PostNL_Topic");

    var tracker = new DistanceTracker(devicePairRepo, deviceRepo, snsRepo, logger);
    await tracker.Notify(vehicles);
  }
}
