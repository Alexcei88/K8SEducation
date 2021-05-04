using System.Collections.Generic;

namespace OTUS.HomeWork.PricingService.Domain.DTO
{
    public class ProductPriceDTO
    {
        public string ProductId { get; set; }
        public string Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }
    }
    
    public class PriceResponseDTO
    {
        public List<ProductPriceDTO> Products { get; set; }

        public decimal SummaryPrice { get; set; }
    }
}
