using OTUS.HomeWork.Eshop.Domain;
using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain
{
    public record CreateOrderDTO
    {
        public Guid UserId { get; set; }

        public SortedSet<OrderItemDTO> Items { get; init; }

        public OrderStatusDTO Status { get; init; }
    }
}
