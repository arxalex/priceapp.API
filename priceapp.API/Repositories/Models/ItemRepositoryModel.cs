namespace priceapp.API.Repositories.Models;

public class ItemRepositoryModel
{
    public int id { get; set; }
    public string? label { get; set; }
    public string? image { get; set; }
    public int category { get; set; }
    public int brand { get; set; }
    public int package { get; set; }
    public string? barcodes { get; set; }
    public double? units { get; set; }
    public double? term { get; set; }
    public string? consist { get; set; }
    public double? calorie { get; set; }
    public double? carbohydrates { get; set; }
    public double? fat { get; set; }
    public double? proteins { get; set; }
    public string? additional { get; set; }
}