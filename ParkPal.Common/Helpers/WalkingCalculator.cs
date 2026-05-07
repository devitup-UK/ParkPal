namespace ParkPal.Common.Helpers;

public static class WalkingCalculator
{
    private const double EarthRadiusKm = 6371.0;
    
    // An average, leisurely theme park walking pace is ~80 meters per minute
    private const double WalkingSpeedMetersPerMinute = 80.0; 
    
    // Theme park paths wind around things. We add 30% to the straight-line distance.
    private const double ThemeParkPathMultiplier = 1.3; 

    public static int GetWalkingTimeMinutes(double lat1, double lon1, double lat2, double lon2)
    {
        // 1. Calculate straight-line distance using Haversine
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        var straightLineDistanceMeters = (EarthRadiusKm * c) * 1000;

        // 2. Apply the pathing multiplier
        var realisticWalkingDistance = straightLineDistanceMeters * ThemeParkPathMultiplier;

        // 3. Convert distance to time and round up to the nearest minute
        var minutes = Math.Ceiling(realisticWalkingDistance / WalkingSpeedMetersPerMinute);
        
        // Failsafe: Even if things are right next to each other, give them at least 1 minute to walk there!
        return Math.Max(1, (int)minutes);
    }

    private static double ToRadians(double angle)
    {
        return Math.PI * angle / 180.0;
    }
}