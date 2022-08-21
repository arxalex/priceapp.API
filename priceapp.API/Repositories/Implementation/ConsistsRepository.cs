using System.Data;
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

    public async Task InsertConsistAsync(ConsistRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@label", model.label, DbType.String);

        const string query = $"insert into {Table} values (DEFAULT, @label)";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task UpdateConsistAsync(ConsistRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", model.id, DbType.Int32);
        parameters.Add("@label", model.label, DbType.String);

        const string query = $"update {Table} set `label` = @label where `id` = @id";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error updating");
        }
    }
}