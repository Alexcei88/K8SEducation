using System;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.EShop.Domain
{
    public record Bucket
    {
        [Key]
        public Guid UserId { get; set; }

        [Key]
        public Guid ProductId { get; init; }

        public int Quantity { get; set; }

    }
}
