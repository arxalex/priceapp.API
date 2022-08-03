using System.Text.Json;
using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Models;
using priceapp.API.ShopServices.Models;

namespace priceapp.API.Utils;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<ItemRepositoryModel, ItemModel>().BeforeMap((s, d) =>
        {
            d.Additional = s.additional != null ? JsonSerializer.Deserialize<object>(s.additional) : null;
            d.Consist = s.consist != null ? JsonSerializer.Deserialize<List<int>>(s.consist) : null;
        });
        CreateMap<ItemModel, ItemRepositoryModel>().BeforeMap((s, d) =>
        {
            d.additional = JsonSerializer.Serialize(s.Additional);
            d.consist = JsonSerializer.Serialize(s.Consist);
            d.barcodes = null;
        });
        CreateMap<ItemSilpoModel, ItemModel>();
        CreateMap<ItemForaModel, ItemModel>();
        CreateMap<ItemAtbModel, ItemModel>();

        CreateMap<ItemExtendedModel, ItemExtendedRepositoryModel>().BeforeMap((s, d) =>
        {
            d.additional = JsonSerializer.Serialize(s.Additional);
            d.consist = JsonSerializer.Serialize(s.Consist);
        });
        ;
        CreateMap<ItemExtendedRepositoryModel, ItemExtendedModel>().BeforeMap((s, d) =>
        {
            d.Additional = s.additional != null ? JsonSerializer.Deserialize<object>(s.additional) : null;
            d.Consist = s.consist != null ? JsonSerializer.Deserialize<List<int>>(s.consist) : null;
        });
        ;

        CreateMap<CategoryModel, CategoryRepositoryModel>();
        CreateMap<CategoryRepositoryModel, CategoryModel>();

        CreateMap<FilialModel, FilialRepositoryModel>();
        CreateMap<FilialRepositoryModel, FilialModel>();

        CreateMap<UserModel, UserRepositoryModel>();
        CreateMap<UserRepositoryModel, UserModel>();
    }
}