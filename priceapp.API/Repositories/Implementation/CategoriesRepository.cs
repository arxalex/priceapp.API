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
            new() {id = categoryId}
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

    public async Task<List<CategoryLinkRepositoryModel>> GetCategoryLinksByShopAsync(int shopId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {TableLinks} where `shopid` = @shopId";
        var parameters = new DynamicParameters();
        parameters.Add("@shopId", shopId, DbType.Int32);

        return (await connection.QueryAsync<CategoryLinkRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<List<CategoryRepositoryModel>> GetCategoriesAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";

        return (await connection.QueryAsync<CategoryRepositoryModel>(query)).ToList();
    }

    public async Task<CategoryRepositoryModel> GetCategoryByShopAndInShopIdAsync(int shopId, int inShopId)
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
}