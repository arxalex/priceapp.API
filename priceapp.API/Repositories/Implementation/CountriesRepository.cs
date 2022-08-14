using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Utils;

namespace priceapp.API.Repositories.Implementation;

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
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $"select * from {Table} where ";
        var parameters = new DynamicParameters();

        var whereQueryKeywords = DatabaseUtil.GetLikeQuery(keywords, "`label`", parameters, "keyword");

        query += whereQueryKeywords;

        return (await connection.QueryAsync<CountryRepositoryModel>(query, parameters)).ToList();
    }
}