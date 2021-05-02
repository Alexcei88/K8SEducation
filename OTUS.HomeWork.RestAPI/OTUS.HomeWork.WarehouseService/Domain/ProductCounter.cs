using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public record ProductCounter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public Guid ProductId { get; set; }

        public long RemainCount { get; set; }

        public long SoldCount { get; set; }
        
        public long ReserveCount { get; set; }        
    }
}