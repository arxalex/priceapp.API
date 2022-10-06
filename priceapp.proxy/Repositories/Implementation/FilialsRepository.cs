using Dapper;
using priceapp.proxy.Repositories.Interfaces;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Utils;
using priceapp.Utils;

namespace priceapp.proxy.Repositories.Implementation;

public class FilialsRepository : IFilialsRepository
{
    private const string Table = "pa_filials_atb";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public FilialsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<AtbFilialRepositoryModel>> GetAtbFilialsAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = @$"select * from {Table}";

        return (await connection.QueryAsync<AtbFilialRepositoryModel>(query)).ToList();
    }

    public async Task InsertOrUpdateAsync(List<AtbFilialRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var tableQuery = DatabaseUtil.GetSelectStatementFromList(models, parameters);

        var query = @$"insert into {Table} select * from (
									select pp.id, 
										p.inshopid, 
										p.city, 
										p.region, 
										p.street, 
										p.house, 
										p.xcord, 
										p.ycord, 
										p.label
									from
									(
									   {tableQuery}
									) p
									left join {Table} pp on p.inshopid = pp.inshopid 
								) as pi
								on duplicate key update 
									inshopid = pi.inshopid, 
									city = pi.city, 
									region = pi.region, 
									street = pi.street, 
									house = pi.house, 
									xcord = pi.xcord, 
									ycord = pi.ycord, 
								    label = pi.label";
        await connection.ExecuteAsync(query, parameters);
    }
}