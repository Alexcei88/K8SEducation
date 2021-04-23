using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.PricingService.Domain
{
    public class ProductDTO
    {
        public string ProductId { get; set; }
        public string Quantity { get; set; }
    }
    
    public class PriceRequestDTO
    {
        public string UserId { get; set; }

        public List<ProductDTO> Products { get; set; }
    }
}
