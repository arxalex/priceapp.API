using System.ComponentModel.DataAnnotations;

namespace priceapp.API.Controllers.Models.Request;

public class LoginRequestModel
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}