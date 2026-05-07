using System;

namespace ParkPal.Common.Helpers;

public static class GeoHelper
{
    /// <summary>
    /// Calculates the distance between two GPS coordinates in Miles.
    /// </summary>
    public static double CalculateDistanceInMiles(double userLat, double userLon, double parkLat, double parkLon)
    {
        // 1. Convert everything from degrees to radians
        var dLat = (parkLat - userLat) * (Math.PI / 180.0);
        var dLon = (parkLon - userLon) * (Math.PI / 180.0);
        
        var radUserLat = userLat * (Math.PI / 180.0);
        var radParkLat = parkLat * (Math.PI / 180.0);

        // 2. The Haversine Math
        var a = Math.Pow(Math.Sin(dLat / 2.0), 2.0) +
                Math.Cos(radUserLat) * Math.Cos(radParkLat) *
                Math.Pow(Math.Sin(dLon / 2.0), 2.0);

        var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        
        // 3. Earth's radius in miles is approx 3958.8 
        // (Change to 6371.0 if you prefer Kilometers!)
        return 3958.8 * c; 
    }
}