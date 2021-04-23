using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public record ReserveProductDTO
    {
        public Guid Id { get; set; }

        public int Count { get; set; }
    }

    public record ReserveProductRequestDTO
    {
        public Guid OrderNumber { get; set; }

        public List<ReserveProductDTO> Products = new List<ReserveProductDTO>();
    }
}
