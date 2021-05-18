using System;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public class OrderLocationDTO
    {
        public string OrderNumber { get; set; }

        public string Address { get; set; }

        public DateTime DeliveryDate { get; set; }
    }
}
