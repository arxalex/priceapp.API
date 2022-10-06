using System.Data;
using Dapper;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;

namespace priceapp.Repositories.Implementation;

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

    public async Task InsertPackageAsync(PackageRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@short", model.@short, DbType.String);

        const string query = $"insert into {Table} values (DEFAULT, @label, @short)";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task UpdatePackageAsync(PackageRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", model.id, DbType.Int32);
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@short", model.@short, DbType.String);

        const string query = $"update {Table} set `label` = @label, `short` = @short where `id` = @id";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error updating");
        }
    }
}