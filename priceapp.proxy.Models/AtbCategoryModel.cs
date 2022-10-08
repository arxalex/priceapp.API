namespace priceapp.proxy.Models;

public class AtbCategoryModel
{
    public int Id { get; set; }
    public int InternalId { get; set; }
    public string Label { get; set; }
    public int? Parent { get; set; }
}