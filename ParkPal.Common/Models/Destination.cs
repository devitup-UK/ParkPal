namespace ParkPal.Common.Models;

public class Destination
{
    public string DestinationId { get; set; }
    public string Name { get; set; }
    public string Timezone { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    

    public List<Park> Parks { get; set; }

    public Destination(string destinationId, string name)
    {
        DestinationId = destinationId;
        Name = name;
        Parks = new List<Park>();
    }
}