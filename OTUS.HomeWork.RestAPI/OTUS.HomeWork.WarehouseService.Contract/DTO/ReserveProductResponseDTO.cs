using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Contract.DTO
{
    public class ReserveProductResultDTO
    {
        public Guid Id { get; set; }

        public long ReserveCount { get; set; } // если неуспешно

        public bool isSuccess { get; set; }

    }

    public class ReserveProductResponseDTO
    {
        public string OrderNumber { get; set; }

        public bool isSuccess { get; set; }

        public List<ReserveProductResultDTO> Products = new List<ReserveProductResultDTO>();
    }
}
