using AutoMapper;
using DTO.Common;
using DTO.Contracts.CategoryComponent;

namespace BlazorUI.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CategoryModel, CategoryAddedEvent>().ReverseMap();
    }
}
