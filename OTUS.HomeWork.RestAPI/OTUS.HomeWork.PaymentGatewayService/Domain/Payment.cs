using System;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.PaymentGatewayService.Domain
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string IdempotanceKey { get; set; }

    }
}