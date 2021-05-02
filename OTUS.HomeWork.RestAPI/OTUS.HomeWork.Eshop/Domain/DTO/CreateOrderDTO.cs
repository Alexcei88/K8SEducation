using OTUS.HomeWork.Eshop.Domain;
using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain
{
    public record CreateOrderDTO
    {        
        public string IdempotencyKey { get; set; }

        public List<OrderItemDTO> Items { get; init; }

    }
}
