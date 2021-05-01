using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.DeliveryService.Domain
{
    public record Delivery
    {
        [Key]
        public Guid OrderNumber { get; set; }

        public string DeliveryAddress { get; init; }

        public List<DeliveryProduct> Products { get; init; }

        public DeliveryLocation Location { get; init; }
    }
}
