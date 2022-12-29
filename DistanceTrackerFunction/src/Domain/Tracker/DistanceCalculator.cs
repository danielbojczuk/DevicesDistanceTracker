using DistanceTrackerFunction.Domain.Devices;

namespace DistanceTrackerFunction.Domain.Tracker;

public static class DistanceCalculator
{
  public static double CalculateDistanceBetweenDevicesInMeters(Device handheld, Device vehicle)
  {
    double handheldLatitude = DistanceCalculator.ConvertToRadians(handheld.Latitude);
    double handheldLongitude = DistanceCalculator.ConvertToRadians(handheld.Longitude);
    double vehicleLatitude = DistanceCalculator.ConvertToRadians(vehicle.Latitude);
    double vehicleLongitude = DistanceCalculator.ConvertToRadians(vehicle.Longitude);


    double distance = 1000 * 6371 * Math.Acos((Math.Sin(handheldLatitude) * Math.Sin(vehicleLatitude)) + Math.Cos(handheldLatitude) * Math.Cos(vehicleLatitude) * Math.Cos(vehicleLongitude - handheldLongitude));
    return distance;
  }

  private static double ConvertToRadians(double value)
  {
    return value / (180 / Math.PI);
  }
}