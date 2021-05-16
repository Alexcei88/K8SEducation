using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Contract.DTO
{
    public class ReserveProductDTO
    {
        public Guid Id { get; set; }

        public int Count { get; set; }
    }

    public class ReserveProductRequestDTO
    {
        public string OrderNumber { get; set; }

        public List<ReserveProductDTO> Products { get; set; } = new List<ReserveProductDTO>();
    }
}
