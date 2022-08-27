namespace priceapp.API.Repositories.Models;

public class PriceRepositoryModel
{
    public int id { get; set; }
    public int itemid { get; set; }
    public int shopid { get; set; }
    public double price { get; set; }
    public int filialid { get; set; }
    public double quantity { get; set; }
    public double? pricefactor { get; set; }
}