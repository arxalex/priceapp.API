using System.Data;
using Dapper;
using priceapp.API.Repositories.Interfaces;

namespace priceapp.API.Repositories.Implementation;

public class TokensRepository : ITokensRepository
{
    private const string Table = "pa_tokens";
    private const string ConfirmEmailTable = "pa_confirm_email";
    private readonly ILogger<TokensRepository> _logger;
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public TokensRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory, ILogger<TokensRepository> logger)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
        _logger = logger;
    }

    public async Task InsertEmailConfirmationTokenAsync(int userId, string token)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.Int32);
        parameters.Add("@token", token, DbType.String);
        const string query = $"insert into {ConfirmEmailTable} values (DEFAULT, @userId, @token)";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            _logger.LogCritical(
                $"UserService: Something went wrong while generating email token for user {userId}");
            throw new DataException("Something went wrong while generating email token");
        }
    }

    public async Task<bool> IsConfirmEmailTokenExistsAsync(int userId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.Int32);

        const string query = $"select * from {ConfirmEmailTable} where `userid` = @userId";
        return (await connection.QueryAsync(query, parameters)).ToList().Count == 1;
    }

    public async Task CloseConfirmEmailTokenAsync(int userId, string token)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.Int32);
        parameters.Add("@token", token, DbType.String);
        const string query = $"delete from {ConfirmEmailTable} where `userid` = @userId and `token` = @token";
        if (await connection.ExecuteAsync(query, parameters) != 1) throw new DataException("Confirmation went wrong");
    }

    public async Task<bool> IsJWTTokenExistsAsync(string token)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var expires = (int) DateTimeOffset.Now.ToUnixTimeSeconds();
        parameters.Add("@token", token, DbType.String);
        parameters.Add("@expires", expires, DbType.Int32);

        const string query = $"select * from {Table} where `token` = @token and `expires` > @expires";
        return (await connection.QueryAsync(query, parameters)).ToList().Count == 1;
    }

    public async Task DeleteTokensForUserAsync(int userId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.Int32);

        const string query = $"delete from {Table} where `userid` = @userId";
        await connection.ExecuteAsync(query, parameters);
    }

    public async Task DeleteTokenAsync(string token)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@token", token, DbType.String);
        const string query = $"delete from {Table} where `token` = @token";
        if (await connection.ExecuteAsync(query, parameters) != 1) throw new DataException("Deleting token went wrong");
    }

    public async Task InsertTokenAsync(int userId, string token, int expires)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.Int32);
        parameters.Add("@token", token, DbType.String);
        parameters.Add("@expires", expires, DbType.Int32);

        const string query = $"insert into {Table} values (DEFAULT, @userId, @token, @expires)";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            _logger.LogCritical(
                $"TokenService: Something went wrong while inserting token for user {userId}");
            throw new DataException("Something went wrong while inserting token");
        }
    }
}