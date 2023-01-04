using System.ComponentModel.DataAnnotations;

namespace ParkPal.API.Models.Requests.Subscription;

public class VoucherRequest
{
    [Required]
    public string Code { get; set; }
}