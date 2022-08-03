using System.ComponentModel.DataAnnotations;

namespace priceapp.API.Controllers.Models.Request;

public class SearchRequestModel
{
    [Required] public string Search { get; set; }
}