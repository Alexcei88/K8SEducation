using AutoMapper;
using OTUS.HomeWork.PricingService.Domain;
using OTUS.HomeWork.PricingService.Domain.DTO;

namespace OTUS.HomeWork.PricingService
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PriceResult, CalculatedPriceResponseDTO>();
            CreateMap<PriceResult.CalculatedProductPrice, Domain.DTO.CalculatedProductPriceDTO>();
        }
    }
}
