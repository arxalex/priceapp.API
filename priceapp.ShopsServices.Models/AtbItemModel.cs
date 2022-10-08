namespace priceapp.ShopsServices.Models;

public class AtbItemModel
{
    public int id { get; set; }
    public int internalid { get; set; }
    public string label { get; set; }
    public string image { get; set; }
    public int category { get; set; }
    public string? categorylabel { get; set; }
    public string? brand { get; set; }
    public string? country { get; set; }
    public double price { get; set; }
    public double quantity { get; set; }
}