using System;

namespace OTUS.HomeWork.WarehouseService.Contract.DTO
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public decimal BasePrice { get; set; }
        
        public string Description { get; set; }

        public long RemainCount { get; set; }
    }
}