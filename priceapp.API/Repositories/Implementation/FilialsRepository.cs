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

    public async Task<List<FilialRepositoryModel>> GetFilialsByLocationAsync(double xCord, double yCord, double radius)
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
}