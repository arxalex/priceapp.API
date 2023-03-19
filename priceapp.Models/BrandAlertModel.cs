namespace priceapp.Models;

public class BrandAlertModel
{
    public int Id { get; set; }
    public int BrandId { get; set; }
    public string Message { get; set; }
    public string Color { get; set; }
}