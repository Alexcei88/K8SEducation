using System;

namespace OTUS.HomeWork.DeliveryService.Contract.DTO
{
    public class DeliveryProductDTO
    {
        public double Weight { get; set; }

        public double Space { get; set; }

        public Guid ProductId { get; set; }

        public string Name { get; set; }
    }
}
