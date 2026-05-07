namespace ParkPal.Common.API.Models.Dtos;

public class UserProfileDto
{
    public int TotalSubmissions { get; set; }
    public DateTime FirstSeenAt { get; set; }
    public int TrustScore { get; set; }
}