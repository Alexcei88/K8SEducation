using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.DeliveryService.Domain
{
    public record Delivery
    {
        [Key]
        public string OrderNumber { get; set; }

        public List<DeliveryProduct> Products { get; init; }

        public DeliveryLocation Location { get; init; }
    }
}
