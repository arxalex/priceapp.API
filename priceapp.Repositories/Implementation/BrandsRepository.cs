using System.Data;
using Dapper;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Utils;

namespace priceapp.Repositories.Implementation;

public class BrandsRepository : IBrandsRepository
{
    private const string Table = "pa_brand";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public BrandsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<BrandRepositoryModel>> GetBrandsAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";
        return (await connection.QueryAsync<BrandRepositoryModel>(query)).ToList();
    }

    public async Task<List<BrandRepositoryModel>> GetBrandsByKeywordsAsync(List<string> keywords)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $"select * from {Table}";
        var parameters = new DynamicParameters();

        if (keywords.Count != 0)
        {
            query += " where " + DatabaseUtil.GetLikeQuery(keywords, "`label`", parameters, "keyword");
        }

        return (await connection.QueryAsync<BrandRepositoryModel>(query, parameters)).ToList();
    }

    public async Task InsertBrandAsync(BrandRepositoryModel model)
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

    public async Task UpdateBrandAsync(BrandRepositoryModel model)
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