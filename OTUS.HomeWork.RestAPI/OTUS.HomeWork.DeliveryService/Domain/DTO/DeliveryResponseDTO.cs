using System;

namespace OTUS.HomeWork.DeliveryService.Domain.DTO
{
    public record DeliveryResponseDTO
    {
        public string OrderNumber { get; init; }

        public string DeliveryAddress { get; set; }

        public DateTime EstimatedDate { get; init; }

        public DateTime ShipmentDate { get; init; }

        public string CourierName { get; set; }
    }
}
