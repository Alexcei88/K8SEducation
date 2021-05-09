using System;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record BucketItemDTO
    {       
        public Guid ProductId { get; init; }       
        public int Quantity { get; init; }
        public decimal Price { get; init; }
    }
}