using System;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public class ShipmentRequestDTO
    {
        public string OrderNumber { get; set; }

        public string DeliveryAddress { get; init; }

    }
}
