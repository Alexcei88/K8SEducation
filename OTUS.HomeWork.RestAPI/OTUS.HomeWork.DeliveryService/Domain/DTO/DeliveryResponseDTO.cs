using System;

namespace OTUS.HomeWork.DeliveryService.Domain.DTO
{
    public record DeliveryResponseDTO
    {
        public string OrderNumber { get; init; }

        public DateTime DeliveryDate { get; init; }

        public DateTime DeliveryDateShipment { get; init; }
    }
}
