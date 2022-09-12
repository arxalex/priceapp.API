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
        CreateMap<PriceRepositoryModel, PriceModel>().ReverseMap();
    }
}