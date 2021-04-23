using System;
using System.Collections.Generic;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public record ShipmentProductDTO
    {
        public Guid Id { get; set; }

        public int Count { get; set; }
    }

    public class ShipmentRequestDTO
    {
        public Guid OrderNumber { get; set; }

        public List<ShipmentProductDTO> Products = new List<ShipmentProductDTO>();
    }
}
