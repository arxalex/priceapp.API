namespace priceapp.Repositories.Models;

public class UserRepositoryModel
{
    public int id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public int role { get; set; }
    public bool @protected { get; set; }
}