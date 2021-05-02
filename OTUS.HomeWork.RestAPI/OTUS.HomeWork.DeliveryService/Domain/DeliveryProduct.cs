using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.DeliveryService.Domain
{
    public record DeliveryProduct
    {
        [Key]
        public string OrderNumber { get; set; }

        [ForeignKey("OrderNumber")]
        public Delivery Delivery { get; init; }

        public double Weight { get; init; }

        public double Space { get; init; }

        [Key]
        public Guid ProductId { get; init; }

        public string Name { get; init; }
    }
}
