namespace priceapp.proxy.Repositories.Models;

public class AtbItemRepositoryModel
{
    public int id { get; set; }
    public int internalid { get; set; }
    public string label { get; set; }
    public string image { get; set; }
    public int category { get; set; }
    public string? brand { get; set; }
    public string? country { get; set; }
}