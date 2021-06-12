using System;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCreated
        : BrokerMessage
    {
        public const string TYPE = "OrderCreated";

        public string UserEmail { get; set; }

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public string BillingAddressId { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; set; }

        public OrderCreated()
        { }

        public OrderCreated(string id)
        {
            Id = id;
        }

    }
}