namespace priceapp.API.Repositories.Models;

public class CategoryRepositoryModel
{
    public int id { get; set; }
    public string label { get; set; }
    public string? image { get; set; }
    public int? parent { get; set; }
    public bool isFilter { get; set; }
}