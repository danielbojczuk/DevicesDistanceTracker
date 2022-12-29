namespace DistanceTrackerFunction.Domain.Devices;

public struct Notification
{
  public double Latitude { get; init; }
  public double Longitude { get; init; }
  public string VehicleId { get; init; }
  public string HandheldId { get; init; }
  public string AlertType
  {
    get
    {
      return "50mApartDelivery";
    }
  }
}
