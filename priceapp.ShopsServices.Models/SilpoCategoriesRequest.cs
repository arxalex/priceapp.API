namespace priceapp.ShopsServices.Models;

public class SilpoCategoriesRequest
{
    public List<SilpoCategories> tree { get; set; }
    public object EComError { get; set; }
}

public class SilpoCategories
{
    public int itemsCount { get; set; }
    public double score { get; set; }
    public int id { get; set; }
    public int? parentId { get; set; }
    public string? name { get; set; }
    public string? iconPath { get; set; }
    public int? order { get; set; }
    public string? mobileAppIconPath { get; set; }
    public string? slug { get; set; }
}