using System.Collections.Generic;

namespace OTUS.HomeWork.PricingService.Domain.DTO
{
    public class PProductDTO
    {
        public string ProductId { get; set; }
        public string Quantity { get; set; }
    }
    
    public class PriceRequestDTO
    {
        public List<PProductDTO> Products { get; set; }
    }
}
