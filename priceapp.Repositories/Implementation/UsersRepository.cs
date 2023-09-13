using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;

namespace priceapp.Repositories.Implementation;

public class UsersRepository : IUsersRepository
{
    private const string Table = "pa_users";
    private readonly ILogger<UsersRepository> _logger;
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public UsersRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory, ILogger<UsersRepository> logger)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
        _logger = logger;
    }

    public async Task<UserRepositoryModel> GetUserByUsernameAsync(string username)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@username", username, DbType.String);
        const string query = $"select * from {Table} where `username` = @username";
        var users = (await connection.QueryAsync<UserRepositoryModel>(query, parameters)).ToList();
        if (users.Count != 1) throw new InvalidDataException($"User with Username {username} does not exsists");

        return users[0];
    }

    public async Task<UserRepositoryModel> GetUserByEmailAsync(string email)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@email", email, DbType.String);
        const string query = $"select * from {Table} where `email` = @email";
        var users = (await connection.QueryAsync<UserRepositoryModel>(query, parameters)).ToList();
        if (users.Count != 1) throw new InvalidDataException($"User with Email {email} does not exists");

        return users[0];
    }

    public async Task RegisterUserAsync(string username, string email, string password, int role)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@username", username, DbType.String);
        parameters.Add("@email", email, DbType.String);
        parameters.Add("@password", BCrypt.Net.BCrypt.HashPassword(password), DbType.String);
        parameters.Add("@role", role, DbType.Int32);

        const string query = $"insert into {Table} values (DEFAULT, @username, @email, @password, @role, DEFAULT)";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            _logger.LogCritical(
                $"UserService: User with username {email} and email {email} registered unsuccesfull");
            throw new IOException("Something went wrong: unable to register");
        }
    }

    public async Task<bool> IsUserExistsAsync(string username, string email)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@username", username, DbType.String);
        parameters.Add("@email", email, DbType.String);

        const string query = $"select * from {Table} where `email` = @email or `username` = @username";
        return (await connection.QueryAsync(query, parameters)).ToList().Count == 1;
    }

    public async Task<bool> IsUserExistsAsync(int id)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);

        const string query = $"select * from {Table} where `id` = @id";
        return (await connection.QueryAsync(query, parameters)).ToList().Count == 1;
    }

    public async Task<UserRepositoryModel> GetUserByIdAsync(int id)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);

        const string query = $"select * from {Table} where `id` = @id";
        var users = (await connection.QueryAsync<UserRepositoryModel>(query, parameters)).ToList();
        if (users.Count != 1) throw new InvalidDataException($"User with Id {id} does not exists");

        return users[0];
    }

    public async Task UpdateUserRole(int id, int role)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);
        parameters.Add("@role", role, DbType.String);
        const string query = $"update {Table} set `role` = @role where `id` = @id";
        if (await connection.ExecuteAsync(query, parameters) != 1) throw new DataException("Updating role went wrong");
    }

    public async Task ChangePasswordAsync(int id, string password)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);
        parameters.Add("@password", BCrypt.Net.BCrypt.HashPassword(password), DbType.String);
        const string query = $"update {Table} set `password` = @password where `id` = @id";
        if (await connection.ExecuteAsync(query, parameters) != 1) throw new DataException("Updating password went wrong");
    }

    public async Task DeleteAsync(UserRepositoryModel user)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", user.id, DbType.Int32);
        parameters.Add("@username", user.username, DbType.String);
        parameters.Add("@email", user.email, DbType.String);
        parameters.Add("@password", user.password, DbType.String);
        parameters.Add("@role", user.role, DbType.String);
        parameters.Add("@protected", user.@protected, DbType.Boolean);

        const string query = $"delete from {Table} where `id` = @id and `username` = @username and `email` = @email and `password` = @password and `role` = @role and `protected` = @protected";
        if (await connection.ExecuteAsync(query, parameters) != 1) throw new DataException("Deleting user went wrong");
    }
}