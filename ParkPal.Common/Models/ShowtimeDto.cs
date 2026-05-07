using System;

namespace ParkPal.Common.Models;

public class ShowtimeDto
{
    public string? Type { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
}