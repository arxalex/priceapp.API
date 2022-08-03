using System.Data;
using Dapper;
using priceapp.API.Repositories.Interfaces;
using priceapp.API.Repositories.Models;
using priceapp.API.Utils;

namespace priceapp.API.Repositories.Implementation;

public class ItemsRepository : IItemsRepository
{
    private const string Table = "pa_items";
    private readonly MySQLDbConnectionFactory _mySqlDbConnectionFactory;

    public ItemsRepository(MySQLDbConnectionFactory mySqlDbConnectionFactory)
    {
        _mySqlDbConnectionFactory = mySqlDbConnectionFactory;
    }

    public async Task<List<ItemRepositoryModel>> GetItemsAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table}";
        return (await connection.QueryAsync<ItemRepositoryModel>(query)).ToList();
    }

    public async Task<ItemRepositoryModel> GetItemByIdAsync(int id)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table} where `id` = @id";

        var parameters = new DynamicParameters();

        parameters.Add("@id", id, DbType.Int32);

        return await connection.QueryFirstAsync<ItemRepositoryModel>(query, parameters);
    }

    public async Task<List<ItemRepositoryModel>> GetItemsByKeywordsAndCategoryAsync(IEnumerable<string> keywords,
        IEnumerable<int> categoryIds)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $"select * from {Table} where ";
        var parameters = new DynamicParameters();

        var whereQueryKeywords = DatabaseUtil.GetLikeQuery(keywords, "`label`", parameters, "keyword");
        var whereQueryCategories = DatabaseUtil.GetInQuery(categoryIds, "`category`");

        query += whereQueryCategories + " AND (" + whereQueryKeywords + ")";

        return (await connection.QueryAsync<ItemRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<List<ItemRepositoryModel>> GetItemsByKeywordsAsync(List<string> keywords)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $"select * from {Table} where ";
        var parameters = new DynamicParameters();

        var whereQueryKeywords = DatabaseUtil.GetLikeQuery(keywords, "`label`", parameters, "keyword");

        query += whereQueryKeywords;

        return (await connection.QueryAsync<ItemRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<List<ItemExtendedRepositoryModel>> GetItemExtendedByCategoriesAsync(IEnumerable<int> categoryIds,
        int from, int to)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $@"
            select i.id, i.label, i.image, i.category, c.label as categoryLabel, i.brand, b.label as brandLabel,
                   i.package, p.label as packageLabel, p.short as packageUnits, i.units, i.term, i.consist, i.calorie,
                   i.carbohydrates, i.fat, i.proteins, i.additional, 
                   min(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMin,
                   max(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMax
            from {Table} i 
            left join pa_categories c on c.id = i.category
            left join pa_brand b on b.id = i.brand
            left join pa_package p on p.id = i.package
            left join pa_prices pr on pr.itemid = i.id
            where pr.price is not null
            and pr.quantity > 0 ";

        var whereQueryCategories = DatabaseUtil.GetInQuery(categoryIds, "c.id");

        query += "and " + whereQueryCategories + @$"
            group by i.id
            order by i.id
            limit {to - from} offset {from};
        ";

        return (await connection.QueryAsync<ItemExtendedRepositoryModel>(query)).ToList();
    }

    public async Task<List<ItemExtendedRepositoryModel>> GetItemsExtendedByCategoriesAndFilialsAsync(
        IEnumerable<int> categoryIds, IEnumerable<int> filialIds, int from, int to)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $@"
            select i.id, i.label, i.image, i.category, c.label as categoryLabel, i.brand, b.label as brandLabel,
                   i.package, p.label as packageLabel, p.short as packageUnits, i.units, i.term, i.consist, i.calorie,
                   i.carbohydrates, i.fat, i.proteins, i.additional, 
                   min(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMin,
                   max(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMax
            from {Table} i 
            left join pa_categories c on c.id = i.category
            left join pa_brand b on b.id = i.brand
            left join pa_package p on p.id = i.package
            left join pa_prices pr on pr.itemid = i.id
            where pr.price is not null
            and pr.quantity > 0 ";

        var whereQueryCategories = DatabaseUtil.GetInQuery(categoryIds, "c.id");
        var whereQueryFilials = DatabaseUtil.GetInQuery(filialIds, "pr.filialid");

        query += "and " + whereQueryCategories + " and " + whereQueryFilials +
                 @$"
            group by i.id
            order by i.id
            limit {to - from} offset {from};
        ";

        return (await connection.QueryAsync<ItemExtendedRepositoryModel>(query)).ToList();
    }

    public async Task<List<ItemExtendedRepositoryModel>> SearchItemsExtendedAsync(List<string> search)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $@"
            select i.id, i.label, i.image, i.category, c.label as categoryLabel, i.brand, b.label as brandLabel,
                   i.package, p.label as packageLabel, p.short as packageUnits, i.units, i.term, i.consist, i.calorie,
                   i.carbohydrates, i.fat, i.proteins, i.additional, 
                   min(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMin,
                   max(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMax
            from {Table} i 
            left join pa_categories c on c.id = i.category
            left join pa_brand b on b.id = i.brand
            left join pa_package p on p.id = i.package
            left join pa_prices pr on pr.itemid = i.id
            where pr.price is not null
            and pr.quantity > 0 ";

        var parameters = new DynamicParameters();
        var whereQueryKeywords = DatabaseUtil.GetLikeQuery(search, "i.label", parameters, "keyword");

        query += "and (" + whereQueryKeywords +
                 @")
            group by i.id
            order by i.id;
        ";

        return (await connection.QueryAsync<ItemExtendedRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<List<ItemExtendedRepositoryModel>> SearchItemsExtendedByLocationAsync(List<string> search,
        IEnumerable<int> filialIds)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $@"
            select i.id, i.label, i.image, i.category, c.label as categoryLabel, i.brand, b.label as brandLabel,
                   i.package, p.label as packageLabel, p.short as packageUnits, i.units, i.term, i.consist, i.calorie,
                   i.carbohydrates, i.fat, i.proteins, i.additional, 
                   min(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMin,
                   max(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMax
            from {Table} i 
            left join pa_categories c on c.id = i.category
            left join pa_brand b on b.id = i.brand
            left join pa_package p on p.id = i.package
            left join pa_prices pr on pr.itemid = i.id
            where pr.price is not null
            and pr.quantity > 0 ";

        var parameters = new DynamicParameters();
        var whereQueryKeywords = DatabaseUtil.GetLikeQuery(search, "i.label", parameters, "keyword");
        var whereQueryFilials = DatabaseUtil.GetInQuery(filialIds, "pr.filialid");

        query += "and (" + whereQueryKeywords + ") and " + whereQueryFilials +
                 @"
            group by i.id
            order by i.id;
        ";

        return (await connection.QueryAsync<ItemExtendedRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<ItemExtendedRepositoryModel> GetItemExtendedAsync(int id)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $@"
            select i.id, i.label, i.image, i.category, c.label as categoryLabel, i.brand, b.label as brandLabel,
                   i.package, p.label as packageLabel, p.short as packageUnits, i.units, i.term, i.consist, i.calorie,
                   i.carbohydrates, i.fat, i.proteins, i.additional, 
                   min(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMin,
                   max(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMax
            from {Table} i 
            left join pa_categories c on c.id = i.category
            left join pa_brand b on b.id = i.brand
            left join pa_package p on p.id = i.package
            left join pa_prices pr on pr.itemid = i.id
            where i.id = @id
            and pr.price is not null
            and pr.quantity > 0 
            group by i.id";

        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);

        return await connection.QueryFirstAsync<ItemExtendedRepositoryModel>(query, parameters);
    }

    public async Task<ItemExtendedRepositoryModel> GetItemExtendedByLocationAsync(int id, IEnumerable<int> filialIds)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $@"
            select i.id, i.label, i.image, i.category, c.label as categoryLabel, i.brand, b.label as brandLabel,
                   i.package, p.label as packageLabel, p.short as packageUnits, i.units, i.term, i.consist, i.calorie,
                   i.carbohydrates, i.fat, i.proteins, i.additional, 
                   min(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMin,
                   max(if(pr.pricefactor is null, pr.price, pr.price * pr.pricefactor)) as priceMax
            from {Table} i 
            left join pa_categories c on c.id = i.category
            left join pa_brand b on b.id = i.brand
            left join pa_package p on p.id = i.package
            left join pa_prices pr on pr.itemid = i.id
            where i.id = @id
            and pr.price is not null
            and pr.quantity > 0 ";

        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);

        var whereQueryFilials = DatabaseUtil.GetInQuery(filialIds, "pr.filialid");

        query += " and " + whereQueryFilials +
                 @"
            group by i.id;
        ";

        return await connection.QueryFirstAsync<ItemExtendedRepositoryModel>(query, parameters);
    }
}