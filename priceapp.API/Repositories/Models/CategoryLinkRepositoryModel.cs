namespace priceapp.API.Repositories.Models;

public class CategoryLinkRepositoryModel
{
    public int id { get; set; }
    public int categoryid { get; set; }
    public int shopid { get; set; }
    public int categoryshopid { get; set; }
    public string shopcategorylabel { get; set; }
}