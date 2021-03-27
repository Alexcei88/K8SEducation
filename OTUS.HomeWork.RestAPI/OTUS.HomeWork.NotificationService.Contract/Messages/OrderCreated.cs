using System;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCreated
        : NotificationMessage
    {
        public const string TYPE = "OrderCreated";

        public Guid UserId { get; set; }

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public string BillingAddressId { get; set; }

        public override string MessageType => TYPE;

    }
}