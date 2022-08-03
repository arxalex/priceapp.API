using System.ComponentModel.DataAnnotations;

namespace priceapp.API.Controllers.Models.Request;

public class SearchAndLocationRequestModel
{
    [Required] public double XCord { get; set; }
    [Required] public double YCord { get; set; }
    [Required] public double Radius { get; set; }
    [Required] public string Search { get; set; }
}