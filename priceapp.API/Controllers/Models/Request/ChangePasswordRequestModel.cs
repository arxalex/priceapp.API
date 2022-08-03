using System.ComponentModel.DataAnnotations;

namespace priceapp.API.Controllers.Models.Request;

public class ChangePasswordRequestModel
{
    [Required] public string PasswordOld { get; set; }
    [Required] public string Password { get; set; }
}