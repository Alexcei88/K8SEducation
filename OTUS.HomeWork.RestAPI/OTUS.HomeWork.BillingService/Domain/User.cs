using System;
using System.ComponentModel.DataAnnotations;

namespace OTUS.HomeWork.BillingService.Domain
{
    internal class User
    {
        [Key]
        public Guid Id { get; set; }

        public decimal Balance { get; set; }
    }
}