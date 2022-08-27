namespace priceapp.API.Models;

public class PriceHistoryModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int ShopId { get; set; }
    public double Price { get; set; }
    public DateTime Date { get; set; }
    public int FilialId { get; set; }
}