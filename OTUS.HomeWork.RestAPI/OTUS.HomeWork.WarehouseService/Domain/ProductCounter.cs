using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.Eshop.Domain
{
    public record ProductCounter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }
       
        public long RemainCount { get; set; }

        public long SoldCount { get; set; }
        
        public long ReserveCount { get; init; }        
    }
}