using System.Collections.Generic;

namespace OTUS.HomeWork.PricingService.Domain.DTO
{
    public class CalculatedProductPriceDTO
    {
        public string ProductId { get; set; }
        public string Quantity { get; set; }
        public decimal Price { get; set; }        
    }
    
    public class CalculatedPriceResponseDTO
    {
        public List<CalculatedProductPriceDTO> Products { get; set; }

        public decimal SummaryPrice { get; set; }

        public decimal Discount { get; set; }
    }
}
