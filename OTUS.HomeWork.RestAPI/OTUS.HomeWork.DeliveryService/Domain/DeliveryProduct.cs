using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.DeliveryService.Domain
{
    public record DeliveryProduct
    {
        [Column(Order = 1)]        
        [Key]
        public string OrderNumber { get; init; }

        [ForeignKey("OrderNumber")]
        public Delivery Delivery { get; init; }

        public double Weight { get; init; }

        public double Space { get; init; }

        [Key]
        [Column(Order = 2)]
        public Guid ProductId { get; init; }

        public string Name { get; init; }
    }
}
