using System.Data;
using Dapper;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Utils;

namespace priceapp.proxy.Repositories.Implementation;

public class CategoriesRepository : ICategoriesRepository
{
    private const string Table = "pa_categories_atb";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public CategoriesRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<AtbCategoryRepositoryModel>> GetAtbChildCategoriesAsync(int categoryId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        const string query = $"select * from {Table} where ";
        var resultByLevel = new List<List<AtbCategoryRepositoryModel>>();
        var i = 1;
        resultByLevel.Add(new List<AtbCategoryRepositoryModel>
        {
            new() { id = categoryId }
        });

        while (resultByLevel[i - 1].Count > 0)
        {
            var queryResult = query + DatabaseUtil.GetInQuery(resultByLevel[i - 1].Select(x => x.id), "`parent`");
            resultByLevel.Add((await connection.QueryAsync<AtbCategoryRepositoryModel>(queryResult)).ToList());
            i++;
        }

        var result = new List<AtbCategoryRepositoryModel>();

        for (var j = 1; j < resultByLevel.Count; j++) result.AddRange(resultByLevel[j]);

        return result;
    }

    public async Task<List<AtbCategoryRepositoryModel>> GetAtbCategoriesAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        const string query = $"select * from {Table}";
        return (await connection.QueryAsync<AtbCategoryRepositoryModel>(query)).ToList();
    }

    public async Task InsertAsync(AtbCategoryRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@internalId", model.internalid, DbType.Int32);
        if (model.parent != null)
        {
            parameters.Add("@parent", model.parent, DbType.Int32);
        }

        var query = @$"insert into {Table} 
                          values (DEFAULT, 
                                  @internalId, 
                                  @label, 
                                  {(model.parent != null ? "@parent" : "DEFAULT")})";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task InsertOrUpdateAsync(List<AtbCategoryRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var tableQuery = DatabaseUtil.GetSelectStatementFromList(models, parameters);

        var query = @$"insert into {Table} select * from (
									select pp.id, 
										p.internalid, 
										p.label, 
										p.parent
									from
									(
									   {tableQuery}
									) p
									left join {Table} pp on p.internalid = pp.internalid 
								) as pi
								on duplicate key update 
									internalid = pi.internalid, 
								    label = pi.label, 
								    parent = pi.parent";
        await connection.ExecuteAsync(query, parameters);
    }

    public async Task<List<AtbCategoryRepositoryModel>> GetAtbBaseCategoriesAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        const string query = $"select * from {Table} where parent is null";
        return (await connection.QueryAsync<AtbCategoryRepositoryModel>(query)).ToList();
    }
}