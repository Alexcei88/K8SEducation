using System;

namespace OTUS.HomeWork.DeliveryService.Domain.DTO
{
    public record DeliveryProductDTO
    {
        public double Weight { get; init; }

        public double Space { get; init; }

        public Guid ProductId { get; init; }

        public string Name { get; init; }
    }
}
