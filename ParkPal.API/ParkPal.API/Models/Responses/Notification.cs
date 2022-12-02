using ParkPal.Common.API.Models.ThemeParkApi;
using ParkPal.Common.Models;
using ParkPal.Common.Models.Database.Entities.Notification;

namespace ParkPal.API.Models.Responses;

public class Notification
{
    public Item? Properties { get; set; }
    public Attraction? Attraction { get; set; }
    public Park? Park { get; set; }
}