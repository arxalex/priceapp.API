using System.Data;
using MySqlConnector;

namespace priceapp.proxy.Repositories;

public class MySQLDbConnectionFactory
{
    private readonly string _connectionString;

    public MySQLDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Connect()
    {
        return new MySqlConnection(_connectionString);
    }
}