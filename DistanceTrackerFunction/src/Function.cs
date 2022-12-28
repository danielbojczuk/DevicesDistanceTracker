using Amazon.Lambda.Core;
using DistanceTrackerFunction.Domain;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DistanceTrackerFunction;

public class Function
{
  public void FunctionHandler(object input, ILambdaContext context)
  {
    //Get old and new data from Vehicle.
    //If position didnt changed, car is stoped, then check the HandHeld position.
    //If difference between vehicle and handheld positions are over 50m, send notification.
    var device = new Device
    {
      Longitude = 1,
      Latitude = 1,
      MacAddress = "macAddress",
      DeviceType = DeviceTypeEnum.Handheld,
      LastUpdated = new DateTime()
    };

    Console.WriteLine(device);
    Console.WriteLine(input);
  }
}
