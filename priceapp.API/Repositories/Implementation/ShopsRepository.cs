using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Implementation;

public class ShopsRepository : IShopsRepository
{
    private const string Table = "pa_shops";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public ShopsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<ShopRepositoryModel>> GetShopsAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";
        return (await connection.QueryAsync<ShopRepositoryModel>(query)).ToList();
    }
}