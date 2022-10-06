using System.Data;
using Dapper;
using priceapp.Repositories.Interfaces;
using priceapp.Repositories.Models;
using priceapp.Utils;

namespace priceapp.Repositories.Implementation;

public class ItemsRepository : IItemsRepository
{
    private const string Table = "pa_items";
    private const string TableLink = "pa_items_link";
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

    public async Task<ItemRepositoryModel> GetItemAsync(int id)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table} where `id` = @id";

        var parameters = new DynamicParameters();

        parameters.Add("@id", id, DbType.Int32);

        return await connection.QueryFirstAsync<ItemRepositoryModel>(query, parameters);
    }

    public async Task<List<ItemRepositoryModel>> GetItemsAsync(IEnumerable<string> keywords,
        IEnumerable<int> categoryIds)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $"select * from {Table}";
        var parameters = new DynamicParameters();
        
        if (keywords.ToList().Count != 0)
        {
            query += " where (" + DatabaseUtil.GetLikeQuery(keywords, "`label`", parameters, "keyword") + ")";
            if (categoryIds.ToList().Count != 0)
            {
                query += " and " + DatabaseUtil.GetInQuery(categoryIds, "`category`");
            }
        }
        else
        {
            if (categoryIds.ToList().Count != 0)
            {
                query += " where " + DatabaseUtil.GetInQuery(categoryIds, "`category`");
            }
        }

        return (await connection.QueryAsync<ItemRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<List<ItemRepositoryModel>> GetItemsAsync(List<string> keywords)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = $"select * from {Table}";
        var parameters = new DynamicParameters();

        if (keywords.Count != 0)
        {
            query += " where " + DatabaseUtil.GetLikeQuery(keywords, "`label`", parameters, "keyword");
        }

        return (await connection.QueryAsync<ItemRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<List<ItemExtendedRepositoryModel>> GetItemExtendedAsync(IEnumerable<int> categoryIds,
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

        if (categoryIds.ToList().Count != 0)
        {
            query += " and " + DatabaseUtil.GetInQuery(categoryIds, "c.id");
        }
        query += $" group by i.id order by i.id limit {to - from} offset {from};";

        return (await connection.QueryAsync<ItemExtendedRepositoryModel>(query)).ToList();
    }

    public async Task<List<ItemExtendedRepositoryModel>> GetItemsExtendedAsync(
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
            and pr.quantity > 0";
        
        if (categoryIds.ToList().Count != 0)
        {
            query += " and " + DatabaseUtil.GetInQuery(categoryIds, "c.id");
        }
        if (filialIds.ToList().Count != 0)
        {
            query += " and " + DatabaseUtil.GetInQuery(filialIds, "pr.filialid");
        }

        query += $" group by i.id order by i.id limit {to - from} offset {from};";

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
        if (search.Count != 0)
        {
            query += " and (" + DatabaseUtil.GetLikeQuery(search, "i.label", parameters, "keyword") + ")";
        }
        
        query += " group by i.id order by i.id;";

        return (await connection.QueryAsync<ItemExtendedRepositoryModel>(query, parameters)).ToList();
    }

    public async Task<List<ItemExtendedRepositoryModel>> SearchItemsExtendedAsync(List<string> search,
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

        if (filialIds.ToList().Count != 0)
        {
            query += " and " + DatabaseUtil.GetInQuery(filialIds, "pr.filialid");
        }

        if (search.Count != 0)
        {
            query += " and (" + DatabaseUtil.GetLikeQuery(search, "i.label", parameters, "keyword") + ")";
        }

        query += " group by i.id order by i.id;";

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

    public async Task<ItemExtendedRepositoryModel> GetItemExtendedAsync(int id, IEnumerable<int> filialIds)
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
        
        if (filialIds.ToList().Count != 0)
        {
            query += " and " + DatabaseUtil.GetInQuery(filialIds, "pr.filialid");
        }

        query += " group by i.id;";

        return await connection.QueryFirstAsync<ItemExtendedRepositoryModel>(query, parameters);
    }

    public async Task<List<ItemLinkRepositoryModel>> GetItemLinksAsync(int shopId)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {TableLink} where `shopid` = @shopId";

        var parameters = new DynamicParameters();

        parameters.Add("@shopId", shopId, DbType.Int32);

        return (await connection.QueryAsync<ItemLinkRepositoryModel>(query, parameters)).ToList();
    }

    public async Task InsertItemAsync(ItemRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@image", model.image, DbType.String);
        parameters.Add("@category", model.category, DbType.Int32);
        parameters.Add("@brand", model.brand, DbType.Int32);
        parameters.Add("@package", model.package, DbType.Int32);
        if (model.units != null)
        {
            parameters.Add("@units", model.units, DbType.Double);
        }

        if (model.term != null)
        {
            parameters.Add("@term", model.term, DbType.Double);
        }

        if (model.barcodes != null)
        {
            parameters.Add("@barcodes", model.barcodes, DbType.String);
        }

        if (model.consist != null)
        {
            parameters.Add("@consist", model.consist, DbType.String);
        }

        if (model.calorie != null)
        {
            parameters.Add("@calorie", model.calorie, DbType.Double);
        }

        if (model.carbohydrates != null)
        {
            parameters.Add("@carbohydrates", model.carbohydrates, DbType.Double);
        }

        if (model.fat != null)
        {
            parameters.Add("@fat", model.fat, DbType.Double);
        }

        if (model.proteins != null)
        {
            parameters.Add("@proteins", model.proteins, DbType.Double);
        }

        if (model.additional != null)
        {
            parameters.Add("@additional", model.additional, DbType.String);
        }


        var query = @$"insert into {Table} 
                          values (DEFAULT, 
                                  @label, 
                                  @image, 
                                  @category, 
                                  @brand, 
                                  @package, 
                                  {(model.units != null ? "@units" : "DEFAULT")}, 
                                  {(model.term != null ? "@term" : "DEFAULT")}
                                  {(model.barcodes != null ? "@barcodes" : "DEFAULT")}, 
                                  {(model.consist != null ? "@consist" : "DEFAULT")}, 
                                  {(model.calorie != null ? "@calorie" : "DEFAULT")}, 
                                  {(model.carbohydrates != null ? "@carbohydrates" : "DEFAULT")}, 
                                  {(model.fat != null ? "@fat" : "DEFAULT")}, 
                                  {(model.proteins != null ? "@proteins" : "DEFAULT")}, 
                                  {(model.additional != null ? "@additional" : "DEFAULT")})";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task UpdateItemAsync(ItemRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@id", model.id, DbType.Int32);
        parameters.Add("@label", model.label, DbType.String);
        parameters.Add("@image", model.image, DbType.String);
        parameters.Add("@category", model.category, DbType.Int32);
        parameters.Add("@brand", model.brand, DbType.Int32);
        parameters.Add("@package", model.package, DbType.Int32);
        if (model.units != null)
        {
            parameters.Add("@units", model.units, DbType.Double);
        }

        if (model.term != null)
        {
            parameters.Add("@term", model.term, DbType.Double);
        }

        if (model.barcodes != null)
        {
            parameters.Add("@barcodes", model.barcodes, DbType.String);
        }

        if (model.consist != null)
        {
            parameters.Add("@consist", model.consist, DbType.String);
        }

        if (model.calorie != null)
        {
            parameters.Add("@calorie", model.calorie, DbType.Double);
        }

        if (model.carbohydrates != null)
        {
            parameters.Add("@carbohydrates", model.carbohydrates, DbType.Double);
        }

        if (model.fat != null)
        {
            parameters.Add("@fat", model.fat, DbType.Double);
        }

        if (model.proteins != null)
        {
            parameters.Add("@proteins", model.proteins, DbType.Double);
        }

        if (model.additional != null)
        {
            parameters.Add("@additional", model.additional, DbType.String);
        }

        var query = @$"update {Table} 
                       set `label` = @label,
                           `image` = @image,
                           `category` = @category,
                           `brand` = @brand,
                           `package` = @package,
                           `units` = {(model.units != null ? "@units" : "null")},
                           `term` = {(model.term != null ? "@term" : "null")},
                           `barcodes` = {(model.barcodes != null ? "@barcodes" : "null")},
                           `consist` = {(model.consist != null ? "@consist" : "null")},
                           `calorie` = {(model.calorie != null ? "@calorie" : "null")},
                           `carbohydrates` = {(model.carbohydrates != null ? "@carbohydrates" : "null")},
                           `fat` = {(model.fat != null ? "@fat" : "null")},
                           `proteins` = {(model.proteins != null ? "@proteins" : "null")},
                           `additional` = {(model.additional != null ? "@additional" : "null")}
                       where `id` = @id";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error updating");
        }
    }

    public async Task InsertItemLinkAsync(ItemLinkRepositoryModel model)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var parameters = new DynamicParameters();
        parameters.Add("@itemid", model.itemid, DbType.Int32);
        parameters.Add("@shopid", model.shopid, DbType.Int32);
        parameters.Add("@inshopid", model.inshopid, DbType.Int32);
        parameters.Add("@pricefactor", model.pricefactor, DbType.Double);

        const string query = $"insert into {TableLink} values (DEFAULT, @itemid, @shopid, @inshopid, @pricefactor)";
        if (await connection.ExecuteAsync(query, parameters) != 1)
        {
            throw new IOException("Error inserting");
        }
    }

    public async Task<ItemRepositoryModel> GetLastInsertedItemAsync()
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        const string query = $"select * from {Table} order by `id` desc";

        return await connection.QueryFirstAsync<ItemRepositoryModel>(query);
    }

    public async Task<List<ItemLinkRepositoryModel>> GetItemLinksAsync(int shopId, IEnumerable<int> categoryIds)
    {
        using var connection = _mySqlDbConnectionFactory.Connect();
        var query = @$"select il.id, il.itemid, il.shopid, il.inshopid, il.pricefactor 
                       from {TableLink} il 
                           left join {Table} i on il.itemid = i.id 
                       where il.shopid = @shopId";

        var parameters = new DynamicParameters();
        parameters.Add("@shopId", shopId, DbType.Int32);
        
        if (categoryIds.ToList().Count != 0)
        {
            query += " and " + DatabaseUtil.GetInQuery(categoryIds, "i.category");
        }

        return (await connection.QueryAsync<ItemLinkRepositoryModel>(query, parameters)).ToList();
    }
}