namespace priceapp.proxy.Repositories.Models;

public class PriceRepositoryModel
{
    public int id { get; set; }
    public int itemid { get; set; }
    public int shopid { get; set; }
    public double price { get; set; }
    public int filialid { get; set; }
    public int quantity { get; set; }
    public int updatetime { get; set; }
}