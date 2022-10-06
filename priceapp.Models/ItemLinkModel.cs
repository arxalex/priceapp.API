namespace priceapp.Models;

public class ItemLinkModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int ShopId { get; set; }
    public int InShopId { get; set; }
    public double PriceFactor { get; set; }
}