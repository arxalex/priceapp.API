namespace priceapp.API.Controllers.Models.Response;

public class LoginResponseModel
{
    public bool Status { get; set; }
    public int Role { get; set; }
    public string? Token { get; set; }
}