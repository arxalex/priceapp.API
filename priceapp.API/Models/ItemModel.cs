namespace priceapp.API.Models;

public class ItemModel
{
    public int Id { get; set; }
    public string Label { get; set; }
    public string Image { get; set; }
    public int Category { get; set; }
    public int Brand { get; set; }
    public int Package { get; set; }
    public double Units { get; set; }
    public double Term { get; set; }
    //public int[]? Consist { get; set; }
    public double? Calorie { get; set; }
    public double? Carbohydrates { get; set; }
    public double? Fat { get; set; }
    public double? Proteins { get; set; }
    public object? Additional { get; set; }
}