using System.ComponentModel.DataAnnotations;

namespace priceapp.API.Controllers.Models.Request;

public class VerifyEmailRequestModel
{
    [Required] public int UserId { get; set; }
    [Required] public string Token { get; set; }
}