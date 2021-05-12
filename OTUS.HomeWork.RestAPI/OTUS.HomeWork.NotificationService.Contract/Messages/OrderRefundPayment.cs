using System;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderRefundPayment
        : NotificationMessage
    {
        public const string TYPE = "OrderRefundPayment";

        public Guid UserId { get; set; }

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public string BillingId { get; set; }

        public override string MessageType => TYPE;

    }
}