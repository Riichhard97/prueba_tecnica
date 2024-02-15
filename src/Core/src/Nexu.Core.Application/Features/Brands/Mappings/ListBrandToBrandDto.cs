using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinqKit;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;

namespace Nexu.Core.Application.Brands.Mappings
{
    public class ListBrandToBrandDto : Profile
    {
        public ListBrandToBrandDto()
        {
            CreateMap<Brand, BrandDto>(MemberList.Destination)
                 .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                 .ForMember(dest => dest.average_price, opt => opt.MapFrom(src => calculateAveragePriceOfBrand(src.Models)));


        }

        public static int? calculateAveragePriceOfBrand(ICollection<Model> models)
        {
            if(models == null)
                return null;

            if (models.Count == 0)
                return null;


            int total = models.Aggregate(0, (accumulator, currentValue) => accumulator + currentValue.AveragePrice);
            return total / models.Count;
        }
    }
}
