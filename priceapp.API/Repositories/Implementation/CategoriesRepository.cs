using System.Data;
using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Utils;

namespace priceapp.API.Repositories.Implementation;

public class CategoriesRepository : ICategoriesRepository
{
    private const string Table = "pa_categories";
    private const string TableLinks = "pa_categories_link";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public CategoriesRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<CategoryRepositoryModel>> GetChildCategoriesAsync(int categoryId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table} where ";
        var resultByLevel = new List<List<CategoryRepositoryModel>>();
        var i = 1;
        resultByLevel.Add(new List<CategoryRepositoryModel>
        {
            new() { id = categoryId }
        });

        while (resultByLevel[i - 1].Count > 0)
        {
            var queryResult = query + DatabaseUtil.GetInQuery(resultByLevel[i - 1].Select(x => x.id), "`parent`");
            resultByLevel.Add((await connection.QueryAsync<CategoryRepositoryModel>(queryResult)).ToList());
            i++;
        }

        var result = new List<CategoryRepositoryModel>();

        for (var j = 1; j < resultByLevel.Count; j++) result.AddRange(resultByLevel[j]);

        return result;
    }

    public async Task<List<CategoryRepositoryModel>> GetCategoriesAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";

        return (await connection.QueryAsync<CategoryRepositoryModel>(query)).ToList();
    }

    public async Task<CategoryRepositoryModel> GetCategoryAsync(int shopId, int inShopId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = @$"select t.id, t.label, t.parent, t.isFilter, t.image 
                                from {Table} t
                                left join {TableLinks} tl on tl.categoryid = t.id
                                where tl.shopid = @shopId
                                and tl.categoryshopid = @categoryshopid
                                limit 1";
        var parameters = new DynamicParameters();
        parameters.Add("@shopId", shopId, DbType.Int32);
        parameters.Add("@categoryshopid", inShopId, DbType.Int32);

        return await connection.QueryFirstAsync<CategoryRepositoryModel>(query, parameters);
    }

    public async Task InsertCategoryAsync(CategoryRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@label", model.label, DbType.String);
        if (model.image != null)
        {
            parameters.Add("@image", model.image, DbType.String);
        }

        if (model.parent != null)
        {
            parameters.Add("@parent", model.parent, DbType.String);
        }

        parameters.Add("@isFilter", model.isFilter, DbType.String);

        var query = @$"insert into {Table} 
                          values (DEFAULT, 
                                  @label, 
                                  {(model.parent != null ? "@parent" : "DEFAULT")}, 
                                  @isFilter, 
                                  {(model.image != null ? "@image" : "DEFAULT")})";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task UpdateCategoryAsync(CategoryRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", model.id, DbType.Int32);
        parameters.Add("@label", model.label, DbType.String);
        if (model.image != null)
        {
            parameters.Add("@image", model.image, DbType.String);
        }

        if (model.parent != null)
        {
            parameters.Add("@parent", model.parent, DbType.String);
        }

        parameters.Add("@isFilter", model.isFilter, DbType.String);

        var query = @$"update {Table} 
                       set `label` = @label,
                           `parent` = {(model.parent != null ? "@parent" : "null")},
                           `isFilter` = @isFilter,
                           `image` = {(model.image != null ? "@image" : "null")}
                       where `id` = @id";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error updating");
        }
    }

    public async Task<List<CategoryRepositoryModel>> GetBaseCategoriesAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table} where `parent` is null";

        return (await connection.QueryAsync<CategoryRepositoryModel>(query)).ToList();
    }
}