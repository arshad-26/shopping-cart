using AutoMapper;
using DAL.Entities;
using DTO.Common;

namespace ShoppingCartAPI.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryModel>().ReverseMap();
        CreateMap<Item, ItemModel>().ReverseMap();
    }
}
