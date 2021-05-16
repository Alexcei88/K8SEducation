using System;

namespace OTUS.HomeWork.DeliveryService.Contract.DTO
{
    public class DeliveryResponseDTO
    {
        public string OrderNumber { get; set; }

        public string DeliveryAddress { get; set; }

        public DateTime EstimatedDate { get; set; }

        public DateTime ShipmentDate { get; set; }

        public string CourierName { get; set; }
    }
}
