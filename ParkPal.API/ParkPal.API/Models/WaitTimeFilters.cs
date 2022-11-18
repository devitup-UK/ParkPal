using ParkPal.API.Models.Enums;

namespace ParkPal.API.Models;

public class WaitTimeFilters
{
    public WaitTimeFilterType Type { get; set; }
    public WaitTimeFilterSort Sort { get; set; }
}