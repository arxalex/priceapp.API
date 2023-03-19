using System.Data;
using Dapper;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Utils;

namespace priceapp.Repositories.Implementation;

public class PricesRepository : IPricesRepository
{
    private const string Table = "pa_prices";
    private const string TableHistory = "pa_prices_history";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public PricesRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task InsertOrUpdatePricesAsync(List<PriceRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        var i = 0;
        var modelGroups = models.GroupBy(x => i++ / 100);
        
        foreach (var modelGroup in modelGroups)
        {
	        var parameters = new DynamicParameters();
	        var tableQuery = DatabaseUtil.GetSelectStatementFromList(modelGroup.ToList(), parameters);

	        var query = @$"insert into {Table} select * from (
									select pp.id, 
										p.itemid, 
										p.shopid, 
										p.price, 
										p.filialid, 
										p.quantity, 
										if(p.pricefactor is null, pp.pricefactor, p.pricefactor) as pricefactor
									from
									(
									   {tableQuery}
									) p
									left join {Table} pp on p.itemid = pp.itemid 
										and p.shopid = pp.shopid
										and p.filialid = pp.filialid
								) as pi
								on duplicate key update 
									itemid = pi.itemid, 
								    shopid = pi.shopid, 
								    price = pi.price, 
								    filialid = pi.filialid, 
								    quantity = pi.quantity,
								    pricefactor = pi.pricefactor";
	        await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task SetPriceQuantitiesZeroAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        const string query = $"update {Table} set `quantity` = 0";
        await connection.ExecuteAsync(query);
    }

    public async Task SetPriceQuantitiesZeroAsync(int filialId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@filialId", filialId, DbType.Int32);

        const string query = $"update {Table} set `quantity` = 0 where `filialid` = @filialId";
        await connection.ExecuteAsync(query, parameters);
    }

    public async Task InsertOrUpdatePricesHistoryAsync(List<PriceHistoryRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        
        var i = 0;
        var modelGroups = models.GroupBy(x => i++ / 100);

        foreach (var modelGroup in modelGroups)
        {
	        var parameters = new DynamicParameters();
	        var tableQuery = DatabaseUtil.GetSelectStatementFromList(modelGroup.ToList(), parameters);

	        var query = @$"insert into {TableHistory} select * from (
									select pp.id, 
										p.itemid, 
										p.shopid, 
										p.price, 
										p.date, 
										p.filialid
									from
									(
									   {tableQuery}
									) p
									left join {TableHistory} pp on p.itemid = pp.itemid 
										and p.shopid = pp.shopid
										and p.filialid = pp.filialid
										and p.date = pp.date
								) as pi
								on duplicate key update 
									itemid = pi.itemid, 
								    shopid = pi.shopid, 
								    price = pi.price, 
								    date = pi.date,
								    filialid = pi.filialid";
	        await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task<int?> GetMaxFilialIdToday()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@date", DateTime.Now, DbType.DateTime);

        const string query = $"select max(`filialid`) from {TableHistory} where `date` = @date";

        return await connection.QueryFirstAsync<int?>(query, parameters);
    }

    public async Task<List<PriceRepositoryModel>> GetPricesAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";

        return (await connection.QueryAsync<PriceRepositoryModel>(query)).ToList();
    }
    
    public async Task<List<PriceRepositoryModel>> GetPricesAsync(int itemId, IEnumerable<int> filialIds)
    {
	    if (!filialIds.Any())
	    {
		    return new List<PriceRepositoryModel>();
	    }
	    
	    using var connection = _mySqlDbConnectionFactory.Connect();
	    var query = $"select * from {Table} where `itemid` = @itemId and " + DatabaseUtil.GetInQuery(filialIds, "`filialid`");

	    var parameters = new DynamicParameters();
	    parameters.Add("@itemId", itemId);

	    return (await connection.QueryAsync<PriceRepositoryModel>(query)).ToList();
    }
    public async Task<List<PriceRepositoryModel>> GetPricesAsync(IEnumerable<int> itemIds, IEnumerable<int> filialIds)
    {
	    if (!itemIds.Any() || !filialIds.Any())
	    {
		    return new List<PriceRepositoryModel>();
	    }
	    using var connection = _mySqlDbConnectionFactory.Connect();
	    var query = $"select * from {Table} where " + 
	                DatabaseUtil.GetInQuery(filialIds, "`filialid`") +
	                " and " +
	                DatabaseUtil.GetInQuery(itemIds, "`itemid`");

	    return (await connection.QueryAsync<PriceRepositoryModel>(query)).ToList();
    }
}