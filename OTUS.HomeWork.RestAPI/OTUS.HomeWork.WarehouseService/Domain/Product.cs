using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public record Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }

        public string Name { get; init; }

        public decimal BasePrice { get; init; }

        public string Description { get; init; }

        public double Weight {get; init;}

        public double Space { get; init; }

    }
}