namespace DistanceTrackerFunction.Domain;

public struct Device
{
  public double Latitude { get; init; }
  public double Longitude { get; init; }
  public string MacAddress { get; init; }
  public DateTime LastUpdated { get; init; }
  public DeviceTypeEnum DeviceType { get; init; }
}
