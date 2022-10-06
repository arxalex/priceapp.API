namespace priceapp.API.ShopServices.Models;

public class AtbFilialModel
{
    public int id { get; set; }
    public int inshopid { get; set; }
    public string city { get; set; }
    public string region { get; set; }
    public string street { get; set; }
    public string house { get; set; }
    public double xcord { get; set; }
    public double ycord { get; set; }
    public string label { get; set; }
}