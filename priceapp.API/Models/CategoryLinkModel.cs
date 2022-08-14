namespace priceapp.API.Models;

public class CategoryLinkModel
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public int ShopId { get; set; }
    public int CategoryShopId { get; set; }
    public string ShopCategoryLabel { get; set; }
}