namespace ParkPal.Common.Models;

public class Location
{
    public string Longitude { get; set; }
    public string Latitude { get; set; }

    public Location(string longitude, string latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }
}