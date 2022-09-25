using System.Text.Json;
using AutoMapper;
using priceapp.proxy.Repositories.Models;
using priceapp.proxy.Services.Models;

namespace priceapp.proxy.Utils;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<AtbCategoryRepositoryModel, AtbCategoryModel>().ReverseMap();
        CreateMap<AtbFilialRepositoryModel, AtbFilialModel>().ReverseMap();
        CreateMap<AtbItemRepositoryModel, AtbItemModel>().ReverseMap();
        CreateMap<PriceRepositoryModel, PriceModel>()
            .ForMember(d => d.UpdateTime, cfg => cfg.MapFrom((claim, _) => 
                DateTimeOffset.FromUnixTimeSeconds(claim.updatetime).UtcDateTime));
        CreateMap<PriceModel, PriceRepositoryModel>()
            .ForMember(d => d.updatetime, cfg => cfg.MapFrom((claim, _) => 
                new DateTimeOffset(claim.UpdateTime).ToUnixTimeSeconds()));
    }
}