using System;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.Eshop.Domain
{
    public record OrderItem
    {
        public Order Order { get; set; }
        
        [Key]
        public string OrderNumberId { get; set; } 
        
        [Key]
        public Guid ProductId { get; init; }
        
        public int Quantity { get; init; }
    }
}