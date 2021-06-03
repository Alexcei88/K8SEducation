using System;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderWasPayment
        : BrokerMessage
    {
        public const string TYPE = "OrderWasPayment";

        public Guid UserId { get; set; }

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public string BillingId { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; }

        public OrderWasPayment()
        {}

        public OrderWasPayment(string id)
        {
            Id = id;
        }

    }
}