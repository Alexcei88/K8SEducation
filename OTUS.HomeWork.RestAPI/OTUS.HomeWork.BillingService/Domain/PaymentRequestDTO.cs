using System;

namespace OTUS.HomeWork.BillingService.Domain
{
    public class PaymentRequestDTO
    {        
        public decimal Amount { get; set; }

        public string IdempotanceKey { get; set; }
    }
}