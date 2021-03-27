using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.Eshop.Domain
{
    public record Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderNumber { get; set; }
       
        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }
        
        public string BillingId { get; set; }

        public OrderStatus Status { get; set; }
        
        public DateTime CreatedOnUtc { get; set; }
        
        public DateTime? PaidDateUtc { get; set; }
        
        public List<OrderItem> Items { get; set; }
    }
}