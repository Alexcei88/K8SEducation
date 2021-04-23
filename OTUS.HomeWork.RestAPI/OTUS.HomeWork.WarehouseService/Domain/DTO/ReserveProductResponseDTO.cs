using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public record ReserveProductResultDTO
    {
        public Guid Id { get; set; }

        public int RemainCount { get; set; } // если неуспешно

        public bool isSuccess { get; set; }

    }

    public record ReserveProductResponseDTO
    {
        public Guid OrderNumber { get; set; }

        public bool isSuccess { get; set; }

        public List<ReserveProductResultDTO> Products = new List<ReserveProductResultDTO>();
    }
}
