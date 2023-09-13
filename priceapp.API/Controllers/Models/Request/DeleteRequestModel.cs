using System.ComponentModel.DataAnnotations;

namespace priceapp.API.Controllers.Models.Request;

public class DeleteRequestModel
{
    [Required] public string Password { get; set; }
}