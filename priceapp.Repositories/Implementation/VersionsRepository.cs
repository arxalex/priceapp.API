using Dapper;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;

namespace priceapp.Repositories.Implementation;

public class VersionsRepository : IVersionsRepository
{
    private const string Table = "pa_versions";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public VersionsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<VersionRepositoryModel> GetMinVersion()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        const string query = $"select * from {Table} where `isminver` = 1";
        return await connection.QueryFirstAsync<VersionRepositoryModel>(query);
    }
}