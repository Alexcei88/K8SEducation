using System;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public class ReserveProduct
    {
        public Guid OrderNumber { get; set; }

        public Guid ProductId { get; set; }

        public int Count { get; set; }
    }
}
