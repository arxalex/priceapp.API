namespace priceapp.Models;

public class CategoryModel
{
    public int Id { get; set; }
    public string Label { get; set; }
    public string? Image { get; set; }
    public int? Parent { get; set; }
    public bool IsFilter { get; set; }
}