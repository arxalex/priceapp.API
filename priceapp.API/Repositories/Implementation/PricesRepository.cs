using System.Data;
using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Utils;

namespace priceapp.API.Repositories.Implementation;

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
        var parameters = new DynamicParameters();
        var tableQuery = DatabaseUtil.GetSelectStatementFromList(models, parameters);

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
									left join pa_prices pp on p.itemid = pp.itemid 
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
        if (await connection.ExecuteAsync(query, parameters) < 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task SetPriceQuantitiesZeroAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();

        const string query = $"update {Table} set `quantity` = 0";
        if (await connection.ExecuteAsync(query) < 1)
        {
            throw new IOException("Error setting");
        }
    }

    public async Task InsertOrUpdatePricesHistoryAsync(List<PriceHistoryRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var tableQuery = DatabaseUtil.GetSelectStatementFromList(models, parameters);

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
									left join pa_prices pp on p.itemid = pp.itemid 
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
        if (await connection.ExecuteAsync(query, parameters) < 1)
        {
            throw new IOException("Error inserting");
        }
    }
}