namespace priceapp.proxy.Models;

public class AtbItemModel
{
    public int Id { get; set; }
    public int InternalId { get; set; }
    public string Label { get; set; }
    public string Image { get; set; }
    public int Category { get; set; }
    public string? Brand { get; set; }
    public string? Country { get; set; }
}