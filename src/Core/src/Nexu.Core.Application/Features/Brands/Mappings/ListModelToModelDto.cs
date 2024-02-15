using AutoMapper;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;

namespace Nexu.Core.Application.Brands.Mappings
{
    public class ListModelToModelDto : Profile
    {
        public ListModelToModelDto()
        {
            CreateMap<Model, ModelDto>(MemberList.Destination)
                 .ForMember(dest => dest.brand_name, opt => opt.MapFrom(src => src.Brand.Name));
        }
    }
}
