using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record CreateOrderDTO
    {        
        public string IdempotencyKey { get; set; }

        public List<OrderItemDTO> Items { get; init; }

        public string DeliveryAddress { get; set; }
    }
}
