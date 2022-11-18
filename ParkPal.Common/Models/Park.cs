using ParkPal.Common.Models.Database.Entities.Notification;

namespace ParkPal.Common.Models;

public class Park
{
    public string ParkId { get; set; }
    public string Name { get; set; }

    public string Image => ParkId + ".jpeg";

    public List<Attraction> Attractions { get; set; }

    public Park(string parkId, string name)
    {
        ParkId = parkId;
        Name = name;
        Attractions = new List<Attraction>();
    }
    
    public bool Hidden
    {
        get
        {
            if(!String.IsNullOrEmpty(ParkId)) {
                switch (ParkId) {
                    case "b070cbc5-feaa-4b87-a8c1-f94cca037a18":
                    case "ead53ea5-22e5-4095-9a83-8c29300d7c63":
                        return true;
                }
            }
            return false;
        }
    }
}