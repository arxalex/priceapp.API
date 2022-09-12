using System.Data;
using Dapper;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Utils;

namespace priceapp.proxy.Repositories.Implementation;

public class PricesRepository : IPricesRepository
{
    private const string TableItems = "pa_items_atb";
    private const string Table = "pa_prices";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public PricesRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<PriceRepositoryModel>> GetPrices(IEnumerable<int> categoryIds, int shopId, int filialId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var whereQueryCategories = DatabaseUtil.GetInQuery(categoryIds, "t.category");

        var query = @$"select tp.id, tp.itemid, tp.shopid, tp.price, tp.filialid, tp.quantity, tp.updatetime
                                from {Table} tp 
                                left join {TableItems} t on tp.itemid = t.id
                                where tp.shopid = @shopId
                                and tp.filialid = @filialId
                                and {whereQueryCategories}
                                order by tp.id";

        var parameters = new DynamicParameters();
        parameters.Add("@filialId", filialId, DbType.Int32);
        parameters.Add("@shopId", shopId, DbType.Int32);
        return (await connection.QueryAsync<PriceRepositoryModel>(query, parameters)).ToList();
    }

    public async Task InsertOrUpdateAsync(List<PriceRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var tableQuery = DatabaseUtil.GetSelectStatementFromList(models, parameters);

        var query = @$"insert into {Table} select * from (
									select pp.id, 
										p.itemid, 
										p.shopid, 
										p.price, 
										p.filialid, 
										p.quantity, 
										p.updatetime 
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
								    updatetime = pi.updatetime";
        await connection.ExecuteAsync(query, parameters);
    }
}