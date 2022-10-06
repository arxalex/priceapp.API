namespace priceapp.Models;

public class PriceModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int ShopId { get; set; }
    public double Price { get; set; }
    public int FilialId { get; set; }
    public double Quantity { get; set; }
    public double? PriceFactor { get; set; }
}