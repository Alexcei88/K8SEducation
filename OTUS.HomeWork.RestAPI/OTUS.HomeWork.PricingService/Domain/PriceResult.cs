using System.Collections.Generic;

namespace OTUS.HomeWork.PricingService.Domain
{
    public class PriceResult
    {
        public class CalculatedProductPrice
        {
            public string ProductId { get; set; }
            public string Quantity { get; set; }
            public decimal Price { get; set; }
        }

        public List<CalculatedProductPrice> Products { get; set; }

        public decimal SummaryPrice { get; set; }

        public decimal Discount { get; set; }
    }
}
