using System;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record OrderItemDTO
    {       
        public Guid ProductId { get; init; }       
        public int Quantity { get; init; }
    }
}