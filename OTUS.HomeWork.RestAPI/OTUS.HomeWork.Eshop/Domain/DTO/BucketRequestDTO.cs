using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record BucketRequestDTO
    {        
        public Guid Id { get; init; }
        public List<OrderItemDTO> Items { get; init; }
    }
}
