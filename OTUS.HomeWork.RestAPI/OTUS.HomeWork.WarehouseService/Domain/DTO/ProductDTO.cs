using System;

namespace OTUS.HomeWork.Eshop.Domain
{
    public record ProductDTO
    {
        public Guid Id { get; init; }
        
        public string Name { get; init; }
        
        public decimal BasePrice { get; init; }
        
        public string Description { get; init; }

        public long RemainCount { get; set; }
    }
}