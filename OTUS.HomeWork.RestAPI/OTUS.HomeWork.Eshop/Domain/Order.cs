using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.Eshop.Domain
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
        
        public SortedSet<OrderItem> Items { get; init; }
    }
}