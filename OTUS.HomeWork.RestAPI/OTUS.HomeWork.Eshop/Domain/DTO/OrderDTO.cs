using System;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record OrderDTO
    {        
        public Guid OrderNumber { get; set; }

        public OrderStatusDTO Status { get; set; }

        public string ErrorDescription { get; set; }

        public string IdempotencyKey { get; set; }
    }
}
