using System.Collections.Generic;

namespace OTUS.HomeWork.DeliveryService.Domain.DTO
{
    public record DeliveryRequestDTO
    {
        public string OrderNumber { get; init; }

        public string DeliveryAddress { get; init; }

        public List<DeliveryProductDTO> Products { get; init; }
    }
}
