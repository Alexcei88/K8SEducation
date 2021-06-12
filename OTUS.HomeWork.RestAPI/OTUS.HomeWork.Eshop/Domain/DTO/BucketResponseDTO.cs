using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record BucketResponseDTO
    {        
        public List<BucketItemDTO> Items { get; init; }

        public decimal SummaryPrice { get; set; }

        public decimal Discount { get; set; }
    }
}
