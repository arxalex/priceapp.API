namespace priceapp.proxy.Services.Models;

public class AtbFilialModel
{
    public int Id { get; set; }
    public int InShopId { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string Street { get; set; }
    public string House { get; set; }
    public double? XCord { get; set; }
    public double? YCord { get; set; }
    public string Label { get; set; }
}