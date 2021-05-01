using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.DeliveryService.Domain
{
    public record DeliveryLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }

        public string OrderNumber { get; init; }

        [ForeignKey("OrderNumber")]        
        public Delivery Delivery { get; init; }

        public string CurrentAddress { get; init; }

        public string EstimatedDate { get; init; }
    }
}
