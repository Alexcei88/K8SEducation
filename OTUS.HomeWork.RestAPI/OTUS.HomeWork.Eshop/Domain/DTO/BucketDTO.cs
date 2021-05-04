using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record BucketDTO
    {        
        public List<OrderItemDTO> Items { get; init; }
    }
}
