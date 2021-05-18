using System;

namespace OTUS.HomeWork.WarehouseService.Contract.DTO
{
    public class ShipmentRequestDTO
    {
        public string OrderNumber { get; set; }

        public string DeliveryAddress { get; set; }

        public Guid UserId { get; set; }
    }
}
