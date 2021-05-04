using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Domain.DTO
{
    public record ReserveProductDTO
    {
        public Guid Id { get; set; }

        public int Count { get; set; }
    }

    public record ReserveProductRequestDTO
    {
        public string OrderNumber { get; set; }

        public List<ReserveProductDTO> Products { get; set; } = new List<ReserveProductDTO>();
    }
}
