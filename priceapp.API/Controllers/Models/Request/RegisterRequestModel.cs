using System.ComponentModel.DataAnnotations;

namespace priceapp.API.Controllers.Models.Request;

public class RegisterRequestModel
{
    [Required] public string Username { get; set; }
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
}