using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using DistanceTrackerFunction.Domain.Devices;
using DistanceTrackerFunction.Domain.Tracker;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DistanceTrackerFunction;

public class Function
{
  public async void FunctionHandler(DynamoDBEvent input, ILambdaContext context)
  {
    var vehicles = VehicleMapper.FromDynamoDbEvents(input);

    var tracker = new DistanceTracker(null, null, null, null);
    await tracker.Notify(vehicles);
  }
}
