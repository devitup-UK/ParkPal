namespace ParkPal.PushEngine.Models;

public class AppleNotification
{
    public ApsPayload Aps { get; set; } = new();

    public class ApsPayload
    {
        public AlertPayload Alert { get; set; } = new();
        public string Sound { get; set; } = "default";
        public int Badge { get; set; }
    }

    public class AlertPayload
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}