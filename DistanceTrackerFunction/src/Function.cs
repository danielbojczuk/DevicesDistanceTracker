using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DistanceTrackerFunction;

public class Function
{
  public void FunctionHandler(object input, ILambdaContext context)
  {
    Console.WriteLine(input);
  }
}
