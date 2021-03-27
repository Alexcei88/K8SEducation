using OTUS.HomeWork.Eshop.Domain;
using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain
{
    public record CreateOrderDTO
    {        
        public List<OrderItemDTO> Items { get; init; }
    }
}
