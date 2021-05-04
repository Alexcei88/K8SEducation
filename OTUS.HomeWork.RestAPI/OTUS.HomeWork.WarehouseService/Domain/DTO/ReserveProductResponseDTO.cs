using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Domain.DTO
{
    public record ReserveProductResultDTO
    {
        public Guid Id { get; set; }

        public long ReserveCount { get; set; } // если неуспешно

        public bool isSuccess { get; set; }

    }

    public record ReserveProductResponseDTO
    {
        public string OrderNumber { get; set; }

        public bool isSuccess { get; set; }

        public List<ReserveProductResultDTO> Products = new List<ReserveProductResultDTO>();
    }
}
