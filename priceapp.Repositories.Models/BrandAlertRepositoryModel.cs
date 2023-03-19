namespace priceapp.Repositories.Models;

public class BrandAlertRepositoryModel
{
    public int id { get; set; }
    public int brandId { get; set; }
    public string message { get; set; }
    public string color { get; set; }
}