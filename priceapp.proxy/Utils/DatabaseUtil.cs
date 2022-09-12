using System.Data;
using System.Reflection;
using Dapper;

namespace priceapp.proxy.Utils;

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

    public static string GetSelectStatementFromList<T>(List<T> models, DynamicParameters parameters)
    {
        if (models.Count < 1)
        {
            return "";
        }

        const BindingFlags bindingFlags = BindingFlags.Public |
                                          BindingFlags.NonPublic |
                                          BindingFlags.Instance |
                                          BindingFlags.Static;

        var fieldsData = new List<(FieldInfo field, string name, DbType type, bool nullable)>();
        var query = "select ";
        var fields = typeof(T).GetFields(bindingFlags);
        foreach (var field in fields)
        {
            var value = field.GetValue(models[0]);
            var typeName = field.FieldType.Name;
            var name = field.Name.Split(new char[] { '<', '>' })[1];
            var nullable = false;
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeName = Nullable.GetUnderlyingType(field.FieldType)?.Name;
                nullable = true;
            }

            var type = typeName switch
            {
                nameof(String) => DbType.String,
                nameof(Double) => DbType.Double,
                nameof(Int32) => DbType.Int32,
                nameof(DateTime) => DbType.DateTime,
                _ => throw new ArgumentOutOfRangeException()
            };
            fieldsData.Add((field, name, type, nullable));

            if (nullable && value == null)
            {
                query += "null as " + name + ", ";
                continue;
            }

            parameters.Add("@" + name + 0, value, type);
            query += "@" + name + 0 + " as " + name + ", ";
        }

        for (var i = 1; i < models.Count; i++)
        {
            query = query[..^2];
            query += @" union all 
                       select ";
            foreach (var fieldData in fieldsData)
            {
                var value = fieldData.field.GetValue(models[i]);
                if (fieldData.nullable && value == null)
                {
                    query += "null, ";
                    continue;
                }

                parameters.Add("@" + fieldData.name + i, value, fieldData.type);
                query += "@" + fieldData.name + i + ", ";
            }
        }

        return query[..^2];
    }
}