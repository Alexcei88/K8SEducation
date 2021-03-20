using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.Eshop.Domain
{
    public record Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }
        
        public string Name { get; init; }
        
        public decimal Price { get; init; }
        
        public string Description { get; init; }
    }
}