using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Utils;

namespace priceapp.API.Repositories.Implementation;

public class CategoriesRepository : ICategoriesRepository
{
    private const string Table = "pa_categories";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public CategoriesRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<CategoryRepositoryModel>> GetChildCategoriesAsync(int categoryId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table} where ";
        var resultByLevel = new List<List<CategoryRepositoryModel>>();
        var i = 1;
        resultByLevel.Add(new List<CategoryRepositoryModel>
        {
            new() {id = categoryId}
        });

        while (resultByLevel[i - 1].Count > 0)
        {
            var queryResult = query + DatabaseUtil.GetInQuery(resultByLevel[i - 1].Select(x => x.id), "`parent`");
            resultByLevel.Add((await connection.QueryAsync<CategoryRepositoryModel>(queryResult)).ToList());
            i++;
        }

        var result = new List<CategoryRepositoryModel>();

        for (var j = 1; j < resultByLevel.Count; j++) result.AddRange(resultByLevel[j]);

        return result;
    }
}