using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.PaymentGatewayService.Domain
{
    public class Refund
    {
        public Guid UserId { get; set; }

        [Key]
        public Guid BillingId { get; set; }

        [ForeignKey("BillingId")]
        public Payment Payment { get; set; }

        public DateTime Date { get; set; }
    }
}