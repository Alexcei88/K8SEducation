using OTUS.HomeWork.Eshop.Domain;
using System;

namespace OTUS.HomeWork.EShop.Domain
{
    public record CreatedOrderDTO
    {        
        public Guid OrderNumber { get; set; }

        public OrderStatusDTO Status { get; set; }

        public string IdempotencyKey { get; set; }
    }
}
