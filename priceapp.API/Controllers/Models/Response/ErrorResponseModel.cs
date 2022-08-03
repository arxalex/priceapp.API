namespace priceapp.API.Controllers.Models.Response;

public class ErrorResponseModel
{
    public bool Status { get; set; }
    public string Message { get; set; }
    public string Code { get; set; }
}