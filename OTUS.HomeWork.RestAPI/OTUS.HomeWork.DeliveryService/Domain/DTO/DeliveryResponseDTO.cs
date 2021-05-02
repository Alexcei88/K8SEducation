using System;

namespace OTUS.HomeWork.DeliveryService.Domain.DTO
{
    public record DeliveryResponseDTO
    {
        public string OrderNumber { get; init; }

        public DateTime EstimatedDate { get; init; }

        public DateTime ShipmentDate { get; init; }
    }
}
