using Dapper;
using priceapp.Repositories.Interfaces;

namespace priceapp.Repositories.Implementation;

public class SystemRepository : ISystemRepository
{
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public SystemRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<bool> IsDbConnected()
    {
        try
        {
            using var connection = _mySqlDbConnectionFactory.Connect();
            const string query = "select * from pa_users";
            return (await connection.QueryAsync(query)).ToList().Count > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}