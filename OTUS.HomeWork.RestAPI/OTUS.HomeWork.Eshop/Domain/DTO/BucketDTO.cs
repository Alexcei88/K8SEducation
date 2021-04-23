using OTUS.HomeWork.Eshop.Domain;
using System.Collections.Generic;

namespace OTUS.HomeWork.EShop.Domain
{
    public record BucketDTO
    {        
        public List<OrderItemDTO> Items { get; init; }
    }
}
