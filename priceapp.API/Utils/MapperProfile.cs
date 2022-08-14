using System.Text.Json;
using AutoMapper;
using priceapp.API.Models;
using priceapp.API.Repositories.Models;

namespace priceapp.API.Utils;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<ItemRepositoryModel, ItemModel>().BeforeMap((s, d) =>
        {
            d.Additional = s.additional != null ? JsonSerializer.Deserialize<object>(s.additional) : null;
            //d.Consist = s.consist != null ? JsonSerializer.Deserialize<int[]>(s.consist) : null;
            //d.Consist = null;
        });
        CreateMap<ItemModel, ItemRepositoryModel>().BeforeMap((s, d) =>
        {
            d.additional = JsonSerializer.Serialize(s.Additional);
            //d.consist = JsonSerializer.Serialize(s.Consist);
            d.barcodes = null;
        });

        CreateMap<ItemExtendedModel, ItemExtendedRepositoryModel>().BeforeMap((s, d) =>
        {
            d.additional = JsonSerializer.Serialize(s.Additional);
            d.consist = JsonSerializer.Serialize(s.Consist);
        });
        CreateMap<ItemExtendedRepositoryModel, ItemExtendedModel>().BeforeMap((s, d) =>
        {
            d.Additional = JsonSerializer.Deserialize<object>(s.additional);
            d.Consist = JsonSerializer.Deserialize<int[]>(s.consist);
        });

        CreateMap<CategoryModel, CategoryRepositoryModel>();
        CreateMap<CategoryRepositoryModel, CategoryModel>();

        CreateMap<FilialModel, FilialRepositoryModel>();
        CreateMap<FilialRepositoryModel, FilialModel>();

        CreateMap<UserModel, UserRepositoryModel>();
        CreateMap<UserRepositoryModel, UserModel>();

        CreateMap<PackageModel, PackageRepositoryModel>();
        CreateMap<PackageRepositoryModel, PackageModel>();

        CreateMap<ShopModel, ShopRepositoryModel>();
        CreateMap<ShopRepositoryModel, ShopModel>();

        CreateMap<CategoryLinkModel, CategoryLinkRepositoryModel>();
        CreateMap<CategoryLinkRepositoryModel, CategoryLinkModel>();

        CreateMap<BrandModel, BrandRepositoryModel>();
        CreateMap<BrandRepositoryModel, BrandModel>();

        CreateMap<ConsistModel, ConsistRepositoryModel>();
        CreateMap<ConsistRepositoryModel, ConsistModel>();

        CreateMap<CountryModel, CountryRepositoryModel>();
        CreateMap<CountryRepositoryModel, CountryModel>();

        CreateMap<ItemLinkModel, ItemLinkRepositoryModel>();
        CreateMap<ItemLinkRepositoryModel, ItemLinkModel>();
    }
}