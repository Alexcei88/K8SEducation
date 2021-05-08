using System;

namespace OTUS.HomeWork.PaymentGatewayService.Domain
{
    public class RefundDTO
    {
        public Guid BillingId { get; set; }
        
        public bool IsSuccess { get; set; }
    }
}