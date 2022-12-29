using DistanceTrackerFunction.Domain;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace DistanceTrackerFunction.Infrastructure.Logger;

public class Logger : ILogger
{
  public void Error(string message, Exception exception, object? data)
  {
    var log = new Log()
    {
      Message = message,
      Severity = "ERROR",
      Object = data,
      Exception = exception
    };
    LambdaLogger.Log(JsonConvert.SerializeObject(log));
  }

  public void Warning(string message, object? data)
  {
    var log = new Log()
    {
      Message = message,
      Severity = "WARNING",
      Object = data
    };
    LambdaLogger.Log(JsonConvert.SerializeObject(log));
  }
}