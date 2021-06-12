using System;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderWasPayment
        : BrokerMessage
    {
        public const string TYPE = "OrderWasPayment";

        public string UserEmail { get; set; }

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public string BillingId { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; set; }

        public OrderWasPayment()
        {}

        public OrderWasPayment(string id)
        {
            Id = id;
        }

    }
}