using System.Data;
using Dapper;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;

namespace priceapp.proxy.Repositories.Implementation;

public class ConstantsRepository : IConstantsRepository
{
    private const string Table = "pa_constants";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public ConstantsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<ConstantRepositoryModel> GetConstantAsync(string label)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        var parameters = new DynamicParameters();
        parameters.Add("@label", label, DbType.String);

        const string query = $"select * from {Table} where `label` = @label";
        return await connection.QueryFirstAsync<ConstantRepositoryModel>(query, parameters);
    }

    public async Task InsertOrUpdateConstantAsync(ConstantRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        var parameters = new DynamicParameters();
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@id", model.id, DbType.Int32);
        parameters.Add("@value", model.value, DbType.String);
        if (model.id == -1)
        {
            const string querySelect = $"select count(`id`) from {Table} where `label` = @label";
            var count = await connection.QueryFirstAsync<int>(querySelect, parameters);

            switch (count)
            {
                case 0:
                {
                    const string queryInsert =
                        $"insert into {Table} (`id`, `label`, `value`) values (@id, @label, @value)";
                    await connection.ExecuteAsync(queryInsert, parameters);
                    break;
                }
                case 1:
                {
                    const string queryUpdate = $"update {Table} set `value` = @value where `label` = @label";
                    await connection.ExecuteAsync(queryUpdate, parameters);
                    break;
                }
                case > 1:
                    throw new DataException(
                        $"More then 1 records in constants table found. Object: id: {model.id}, label: {model.label}, value: {model.value}");
            }
        }
        else
        {
            const string queryUpdate = $"update {Table} set `value` = @value, `label` = @label or `id` = @id";
            await connection.ExecuteAsync(queryUpdate, parameters);
        }
    }
}