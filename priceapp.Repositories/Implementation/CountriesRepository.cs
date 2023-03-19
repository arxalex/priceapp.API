using System.Data;
using Dapper;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Utils;

namespace priceapp.Repositories.Implementation;

public class CountriesRepository : ICountriesRepository
{
    private const string Table = "pa_countries";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public CountriesRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<CountryRepositoryModel>> GetCountriesAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";
        return (await connection.QueryAsync<CountryRepositoryModel>(query)).ToList();
    }

    public async Task<List<CountryRepositoryModel>> GetCountriesByKeywordsAsync(List<string> keywords)
    {
        if (!keywords.Any())
        {
            return new List<CountryRepositoryModel>();
        }
        
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        var query = $"select * from {Table} where " + DatabaseUtil.GetLikeQuery(keywords, "`label`", parameters, "keyword");

        return (await connection.QueryAsync<CountryRepositoryModel>(query, parameters)).ToList();
    }

    public async Task InsertCountryAsync(CountryRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@short", model.@short, DbType.String);

        const string query = $"insert into {Table} values (DEFAULT, @label, @short)";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task UpdateCountryAsync(CountryRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", model.id, DbType.Int32);
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@short", model.@short, DbType.String);

        const string query = $"update {Table} set `label` = @label, `short` = @short where `id` = @id";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error updating");
        }
    }
}