using System;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderWasPayment
        : NotificationMessage
    {
        public const string TYPE = "OrderWasPayment";

        public Guid UserId { get; set; }

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public string BillingId { get; set; }

        public override string MessageType => TYPE;

    }
}