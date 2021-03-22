using System;

namespace OTUS.HomeWork.Eshop.Domain
{
    public record OrderItemDTO
    {       
        public Guid ProductId { get; init; }       
        public int Quantity { get; init; }
    }
}