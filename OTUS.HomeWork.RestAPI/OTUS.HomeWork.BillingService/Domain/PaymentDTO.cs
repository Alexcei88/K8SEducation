using System;

namespace OTUS.HomeWork.BillingService.Domain
{
    public class PaymentDTO
    {
        public Guid Id { get; set; }
        
        public bool IsSuccess { get; set; }
    }
}