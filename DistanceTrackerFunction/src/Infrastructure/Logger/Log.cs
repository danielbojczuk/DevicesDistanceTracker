namespace DistanceTrackerFunction.Infrastructure.Logger;

public struct Log
{
  public string Severity { get; init; }
  public string Message { get; init; }
  public Exception? Exception { get; init; }
  public object? Object { get; init; }
}
