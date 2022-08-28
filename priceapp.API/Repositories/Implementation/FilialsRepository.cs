using System.Data;
using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Utils;

namespace priceapp.API.Repositories.Implementation;

public class FilialsRepository : IFilialsRepository
{
    private const string Table = "pa_filials";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public FilialsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<FilialRepositoryModel>> GetFilialsAsync(double xCord, double yCord, double radius)
    {
        const double earthRadius = 6372.795;
        var deltaCord = (float) radius / (earthRadius * Math.Cos(yCord * 2 * Math.PI / 360) * 2.0 * Math.PI) * 360.0;
        var minXCord = xCord - deltaCord > -180 ? xCord - deltaCord : xCord - deltaCord + 360;
        var maxXCord = xCord + deltaCord <= 180 ? xCord + deltaCord : xCord + deltaCord - 360;
        var minYCord = yCord - deltaCord > -90 ? yCord - deltaCord : -90;
        var maxYCord = yCord + deltaCord <= 90 ? yCord + deltaCord : 90;

        using var connection = _mySqlDbConnectionFactory.Connect();

        var parameters = new DynamicParameters();
        parameters.Add("@minXCord", minXCord, DbType.Double);
        parameters.Add("@maxXCord", maxXCord, DbType.Double);
        parameters.Add("@minYCord", minYCord, DbType.Double);
        parameters.Add("@maxYCord", maxYCord, DbType.Double);

        const string query = $@"
                    select * from {Table} 
                    where `xcord` > @minXCord 
                      AND `xcord` < @maxXCord 
                      AND `ycord` > @minYCord 
                      AND `ycord` < @maxYCord;
            ";

        var filials = await connection.QueryAsync<FilialRepositoryModel>(query, parameters);
        return filials.Where(filial => LocationUtil.GetLength(xCord, yCord, filial.xcord, filial.ycord) <= radius)
            .ToList();
    }

    public async Task<List<FilialRepositoryModel>> GetFilialsAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";

        return (await connection.QueryAsync<FilialRepositoryModel>(query)).ToList();
    }

    public async Task<List<FilialRepositoryModel>> GetFilialsAsync(int shopId)
    {
	    using var connection = _mySqlDbConnectionFactory.Connect();
	    const string query = $"select * from {Table} where `shopid` = @shopId";
	    var parameters = new DynamicParameters();
	    parameters.Add("@shopId", shopId, DbType.Int32);

	    return (await connection.QueryAsync<FilialRepositoryModel>(query)).ToList();
    }

    public async Task InsertFilialsAsync(List<FilialRepositoryModel> models)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var tableQuery = DatabaseUtil.GetSelectStatementFromList(models, parameters);

        var query = @$"insert into {Table}
							    select null, 
									    t.shopid, 
									    t.inshopid, 
										t.city, 
										t.region, 
										t.street,
										t.house,
										t.xcord,
										t.ycord,
										t.label
								from
								(
							        {tableQuery}
								) t";

        await connection.ExecuteAsync(query, parameters);
    }

    public async Task<string> GetRegionAsync(string city)
    {
	    using var connection = _mySqlDbConnectionFactory.Connect();
	    const string query = @$"select region from {Table} 
								where `city` like @city 
								  and `region` is not null 
								  and `region` <> '' 
								limit 1";
	    var parameters = new DynamicParameters();
	    parameters.Add("@city", $"%{city}%", DbType.String);

	    return await connection.QueryFirstAsync<string>(query);
    }
}