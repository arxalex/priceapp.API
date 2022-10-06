namespace priceapp.Repositories.Models;

public class ItemLinkRepositoryModel
{
    public int id { get; set; }
    public int itemid { get; set; }
    public int shopid { get; set; }
    public int inshopid { get; set; }
    public double pricefactor { get; set; }
}