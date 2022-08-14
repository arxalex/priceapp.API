using System.Data;
using Dapper;

namespace priceapp.API.Utils;

public static class DatabaseUtil
{
    public static string GetLikeQuery(IEnumerable<string> likeArray, string field, DynamicParameters dynamicParameters,
        string parameterPrefix = "like")
    {
        var i = 0;
        var query = "";

        foreach (var value in likeArray)
        {
            dynamicParameters.Add($"@{parameterPrefix}{i}", "%" + value + "%", DbType.String);
            query += $"{field} like @{parameterPrefix}{i++} OR ";
        }
        
        return query[..^3];
    }

    public static string GetInQuery(IEnumerable<int> inArray, string field)
    {
        var query = $"{field} in (";
        query = inArray.Aggregate(query, (current, value) => current + $"{value}, ");
        query = query[..^2];
        query += ")";
        return query;
    }
}