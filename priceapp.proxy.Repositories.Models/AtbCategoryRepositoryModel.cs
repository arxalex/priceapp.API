namespace priceapp.proxy.Repositories.Models;

public class AtbCategoryRepositoryModel
{
    public int id { get; set; }
    public int internalid { get; set; }
    public string label { get; set; }
    public int? parent { get; set; }
}