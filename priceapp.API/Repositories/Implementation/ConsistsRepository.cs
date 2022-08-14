using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Implementation;

public class ConsistsRepository : IConsistsRepository
{
    private const string Table = "pa_consists";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public ConsistsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<ConsistRepositoryModel>> GetConsistsAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";
        return (await connection.QueryAsync<ConsistRepositoryModel>(query)).ToList();
    }
}