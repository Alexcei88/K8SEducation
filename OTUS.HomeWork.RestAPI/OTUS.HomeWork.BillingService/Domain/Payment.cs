using System;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.BillingService.Domain
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

    }
}