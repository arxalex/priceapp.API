using System.Data;
using MySqlConnector;

namespace priceapp.Repositories;

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