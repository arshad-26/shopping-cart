using AutoMapper;
using DTO.Common;

namespace BlazorUI.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ItemModel, CartModel>()
            .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Name));
    }
}
