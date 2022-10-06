namespace priceapp.Models;

public class ItemShopModel
{
    public ItemModel Item { get; set; }
    public int InShopId { get; set; }
    public string? Brand { get; set; }
    public string? Package { get; set; }
    public string? Country { get; set; }
    public string? Category { get; set; }
    public string Url { get; set; }
    public int ShopId { get; set; }
}