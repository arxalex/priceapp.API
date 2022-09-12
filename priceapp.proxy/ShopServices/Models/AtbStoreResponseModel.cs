namespace priceapp.proxy.ShopServices.Models;

public class AtbStoreResponseModel
{
    public int res { get; set; }
    public List<AtbCoordinates> coordinates { get; set; }
    public string optselect { get; set; }
}

public class AtbCoordinates
{
    public int id { get; set; }
    public string lat { get; set; }
    public string lng { get; set; }
}