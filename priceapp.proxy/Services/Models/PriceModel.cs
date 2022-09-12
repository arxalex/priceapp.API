namespace priceapp.proxy.Services.Models;

public class PriceModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int ShopId { get; set; }
    public double Price { get; set; }
    public int FilialId { get; set; }
    public int Quantity { get; set; }
    public DateTime UpdateTime { get; set; }
}