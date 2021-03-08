using System;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.RestAPI.Domain
{
    public record Order
    {
        [Key]
        public string OrderNumber { get; init; }
        
        public decimal TotalPrice { get; init; }
        
        public int BillingId { get; init; }

        public OrderStatus Status { get; init; }
        
        public DateTime CreatedOnUtc { get; init; }
        
        public DateTime? PaidDateUtc { get; init; }
    }
}