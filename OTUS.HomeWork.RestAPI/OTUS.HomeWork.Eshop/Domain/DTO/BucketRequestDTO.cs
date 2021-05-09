using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record BucketRequestDTO
    {        
        public List<OrderItemDTO> Items { get; init; }
    }
}
