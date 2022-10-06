namespace priceapp.Repositories.Models;

public class PriceHistoryRepositoryModel
{
    public int id { get; set; }
    public int itemid { get; set; }
    public int shopid { get; set; }
    public double price { get; set; }
    public DateTime date { get; set; }
    public int filialid { get; set; }
}