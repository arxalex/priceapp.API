using System.Data;
using Dapper;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Utils;
using priceapp.Utils;

namespace priceapp.proxy.Repositories.Implementation;

public class ItemsRepository : IItemsRepository
{
    private const string Table = "pa_items_atb";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public ItemsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<AtbItemRepositoryModel>> GetAtbItemsAsync(IEnumerable<int> categoryIds, int from, int to)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        var query = $"select * from {Table}";
        
        if (categoryIds.ToList().Count != 0)
        {
	        query += " where " + DatabaseUtil.GetInQuery(categoryIds, "category");
        }

        query += " order by id limit @limit offset @offset";

        var parameters = new DynamicParameters();
        parameters.Add("@limit", to - from, DbType.Int32);
        parameters.Add("@offset", from, DbType.Int32);
        return (await connection.QueryAsync<AtbItemRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<List<AtbItemRepositoryModel>> GetAtbItemsAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        const string query = @$"select * from {Table}";
        return (await connection.QueryAsync<AtbItemRepositoryModel>(query)).ToList();
    }

    public async Task InsertAsync(AtbItemRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@internalId", model.internalid, DbType.Int32);
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@image", model.image, DbType.String);
        parameters.Add("@category", model.category, DbType.Int32);
        if (model.brand != null)
        {
            parameters.Add("@brand", model.brand, DbType.String);
        }
        if (model.country != null)
        {
            parameters.Add("@country", model.country, DbType.String);
        }

        var query = @$"insert into {Table} 
                          values (DEFAULT, 
                                  @internalId, 
                                  @label, 
                                  @image, 
                                  @category, 
                                  {(model.brand != null ? "@brand" : "DEFAULT")},
                                  {(model.country != null ? "@country" : "DEFAULT")})";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task InsertOrUpdateAsync(List<AtbItemRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var tableQuery = DatabaseUtil.GetSelectStatementFromList(models, parameters);

        var query = @$"insert into {Table} select * from (
									select pp.id, 
										p.internalid, 
										p.label, 
										p.image, 
										p.category, 
										p.brand, 
										p.country
									from
									(
									   {tableQuery}
									) p
									left join {Table} pp on p.internalid = pp.internalid 
								) as pi
								on duplicate key update 
									internalid = pi.internalid, 
									label = pi.label, 
									image = pi.image, 
									category = pi.category, 
									brand = pi.brand, 
									country = pi.country";
        await connection.ExecuteAsync(query, parameters);
    }
}