using System.Data;
using Dapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Utils;

namespace priceapp.API.Repositories.Implementation;

public class CategoryLinksRepository : ICategoryLinksRepository
{
    private const string TableLinks = "pa_categories_link";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public CategoryLinksRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<CategoryLinkRepositoryModel>> GetCategoryLinksAsync(int shopId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {TableLinks} where `shopid` = @shopId";
        var parameters = new DynamicParameters();
        parameters.Add("@shopId", shopId, DbType.Int32);

        return (await connection.QueryAsync<CategoryLinkRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<CategoryLinkRepositoryModel> GetCategoryLinkAsync(int shopId, int inShopId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = @$"select * 
                                from {TableLinks}
                                where shopid = @shopId
                                and categoryshopid = @categoryshopid
                                limit 1";
        var parameters = new DynamicParameters();
        parameters.Add("@shopId", shopId, DbType.Int32);
        parameters.Add("@categoryshopid", inShopId, DbType.Int32);

        return await connection.QueryFirstAsync<CategoryLinkRepositoryModel>(query, parameters);
    }

    public async Task InsertCategoryLinkAsync(CategoryLinkRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@categoryshopid", model.categoryshopid, DbType.Int32);
        parameters.Add("@shopcategorylabel", model.shopcategorylabel, DbType.String);
        parameters.Add("@shopid", model.shopid, DbType.Int32);
        if (model.categoryid != null)
        {
            parameters.Add("@categoryid", model.categoryid, DbType.Int32);
        }

        var query = @$"insert into {TableLinks} 
                       values (DEFAULT, 
                               {(model.categoryid != null ? "@categoryid" : "DEFAULT")}, 
                               @shopid, 
                               @categoryshopid,
                               @shopcategorylabel)";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task UpdateCategoryLinkAsync(CategoryLinkRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", model.id, DbType.Int32);
        parameters.Add("@categoryshopid", model.categoryshopid, DbType.Int32);
        parameters.Add("@shopcategorylabel", model.shopcategorylabel, DbType.String);
        parameters.Add("@shopid", model.shopid, DbType.Int32);
        if (model.categoryid != null)
        {
            parameters.Add("@categoryid", model.categoryid, DbType.Int32);
        }

        var query = @$"update {TableLinks} 
                       set `categoryid` = {(model.categoryid != null ? "@categoryid" : "null")},
                           `shopid` = @shopid,
                           `categoryshopid` = @categoryshopid,
                           `shopcategorylabel` = @shopcategorylabel,
                       where `id` = @id";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error updating");
        }
    }

    public async Task<List<CategoryLinkRepositoryModel>> GetCategoryLinksAsync(int shopId, int categoryId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {TableLinks} where `shopid` = @shopId and `categoryid` = @categoryId";
        var parameters = new DynamicParameters();
        parameters.Add("@shopId", shopId, DbType.Int32);
        parameters.Add("@categoryId", categoryId, DbType.Int32);

        return (await connection.QueryAsync<CategoryLinkRepositoryModel>(query, parameters)).ToList();
    }

    public async Task InsertOrUpdateCategoryLinksAsync(List<CategoryLinkRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var tableQuery = DatabaseUtil.GetSelectStatementFromList(models, parameters);

        var query = @$"insert into {TableLinks} select * from (
									select pp.id, 
										p.categoryid, 
										p.shopid, 
										p.categoryshopid, 
										p.shopcategorylabel
									from
									(
									   {tableQuery}
									) p
									left join {TableLinks} pp on p.categoryshopid = pp.categoryshopid 
								) as pi
								on duplicate key update 
									categoryid = pi.categoryid, 
								    shopid = pi.shopid, 
								    categoryshopid = pi.categoryshopid, 
								    shopcategorylabel = pi.shopcategorylabel";
        await connection.ExecuteAsync(query, parameters);
    }
}