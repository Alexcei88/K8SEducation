using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public enum ShipmentOrderStatus
    {
        Created = 0, // создана заявка
        DeliveryConfirmed = 1, // заявка подтверждена
        Shipped = 2, // отгружено
        ErrorShipment = 3,
    }

    public class ShipmentOrder
    {
        [Key]
        public string OrderNumber { get; set; }

        public string DeliveryAddress { get; set; }

        public List<Guid> ProductIds { get; set; }

        public ShipmentOrderStatus Status { get; set; }

        public bool WasCancelled { get; set; }

        public DateTime ReadyToShipmentDate { get; set; }

        public DateTime ShipmentDate { get; set; }

        public string ErrorDescription { get; set; }
    }
}
