using System.ComponentModel.DataAnnotations;

namespace priceapp.API.Controllers.Models.Request;

public class LocationRequestModel
{
    [Required] public double XCord { get; set; }
    [Required] public double YCord { get; set; }
    [Required] public double Radius { get; set; }
}