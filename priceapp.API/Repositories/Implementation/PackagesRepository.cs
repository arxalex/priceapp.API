using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;

namespace priceapp.API.Repositories.Implementation;

public class PackagesRepository : IPackagesRepository
{
    private const string Table = "pa_package";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public PackagesRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<PackageRepositoryModel>> GetPackagesAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";
        return (await connection.QueryAsync<PackageRepositoryModel>(query)).ToList();
    }
}