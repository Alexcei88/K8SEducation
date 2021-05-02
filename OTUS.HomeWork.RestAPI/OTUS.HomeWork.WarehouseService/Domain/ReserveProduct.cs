using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.WarehouseService.Domain
{
    public class ReserveProduct
    {
        [Key]
        public string OrderNumber { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Key]
        public Guid ProductId { get; set; }

        public long Count { get; set; }

        public DateTime ReserveDate { get; set; }         // долгозарезервированные товары автоматически снимаются с регистрации
    }
}
