using System.Text.Json;
using AutoMapper;
using priceapp.Models;
using priceapp.Repositories.Models;

namespace priceapp.Services;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<BrandModel, BrandRepositoryModel>().ReverseMap();
        CreateMap<CategoryLinkModel, CategoryLinkRepositoryModel>().ReverseMap();
        CreateMap<CategoryModel, CategoryRepositoryModel>().ReverseMap();
        CreateMap<ConsistModel, ConsistRepositoryModel>().ReverseMap();
        CreateMap<CountryModel, CountryRepositoryModel>().ReverseMap();
        CreateMap<FilialModel, FilialRepositoryModel>().ReverseMap();
        CreateMap<ItemExtendedModel, ItemExtendedRepositoryModel>().BeforeMap((s, d) =>
        {
            d.additional = JsonSerializer.Serialize(s.Additional);
            d.consist = JsonSerializer.Serialize(s.Consist);
        });
        CreateMap<ItemExtendedRepositoryModel, ItemExtendedModel>().BeforeMap((s, d) =>
        {
            d.Additional = s.additional != null ? JsonSerializer.Deserialize<object>(s.additional) : null;
        }).ForMember(d => d.Consist, cfg => cfg.MapFrom((claim, _) =>
            claim.consist != null ? JsonSerializer.Deserialize<List<int>>(claim.consist) : null));
        CreateMap<ItemLinkModel, ItemLinkRepositoryModel>().ReverseMap();
        CreateMap<ItemRepositoryModel, ItemModel>()
            .BeforeMap((s, d) =>
            {
                d.Additional = s.additional != null ? JsonSerializer.Deserialize<object>(s.additional) : null;
            }).ForMember(d => d.Consist, cfg => cfg.MapFrom((claim, _) =>
                claim.consist != null ? JsonSerializer.Deserialize<List<int>>(claim.consist) : null));

        CreateMap<ItemModel, ItemRepositoryModel>().BeforeMap((s, d) =>
        {
            d.additional = JsonSerializer.Serialize(s.Additional);
            d.consist = JsonSerializer.Serialize(s.Consist);
            d.barcodes = null;
        });
        CreateMap<PackageModel, PackageRepositoryModel>().ReverseMap();
        CreateMap<PriceModel, PriceRepositoryModel>().ReverseMap();
        CreateMap<PriceHistoryModel, PriceHistoryRepositoryModel>().ReverseMap();
        CreateMap<ShopModel, ShopRepositoryModel>().ReverseMap();
        CreateMap<UserModel, UserRepositoryModel>().ReverseMap();
        CreateMap<BrandAlertModel, BrandAlertRepositoryModel>().ReverseMap();
    }
}