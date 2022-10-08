namespace priceapp.ShopsServices.Models;

public class SilpoFilialModel
{
    public string title { get; set; }
    public string city { get; set; }
    public string address { get; set; }
    public int id { get; set; }
    public bool pickup { get; set; }
    public bool delivery { get; set; }
    public int businessId { get; set; }
    public string coordinates { get; set; }
    public double lat { get; set; }
    public double lon { get; set; }
    public string allCityNames { get; set; }
    public string openAt { get; set; }
    public string closeAt { get; set; }
}

public class SilpoFilialResponse
{
    public List<SilpoFilialModel> items { get; set; }
    public object EComError { get; set; }
    public List<object> mailBoxes { get; set; }
}