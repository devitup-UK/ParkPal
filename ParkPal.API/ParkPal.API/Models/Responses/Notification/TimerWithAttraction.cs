using ParkPal.Common.API.Models.ThemeParkApi;
using ParkPal.Common.Models;
using ParkPal.Common.Models.Database.Entities.Notification;

namespace ParkPal.API.Models.Responses.Notification;

public class TimerWithAttraction
{
    public AttractionTimer Timer { get; set; }
    public Attraction Attraction { get; set; }
    public Park Park { get; set; }
}