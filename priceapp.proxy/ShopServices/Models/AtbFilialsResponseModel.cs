namespace priceapp.proxy.ShopServices.Models;

public class AtbFilialsResponseModel
{
    public AtbAddress @out { get; set; }
    public string @in { get; set; }
    public bool atbdelivery { get; set; }
}

public class AtbAddress
{
    public string city { get; set; }
    public string address { get; set; }
}