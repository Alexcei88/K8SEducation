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
    }

    public class ShipmentOrder
    {
        [Key]
        public string OrderNumber { get; set; }

        public string DeliveryAddress { get; set; }

        public List<Guid> ProductIds { get; set; }

        public ShipmentOrderStatus Status { get; set; }
    }
}
