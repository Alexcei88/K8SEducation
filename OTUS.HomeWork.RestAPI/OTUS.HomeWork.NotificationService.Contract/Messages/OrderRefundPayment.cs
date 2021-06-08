using System;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderRefundPayment
        : BrokerMessage
    {
        public const string TYPE = "OrderRefundPayment";

        public Guid UserId { get; set; }

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public string BillingId { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; set; }

        public OrderRefundPayment()
        { }

        public OrderRefundPayment(string id)
        {
            Id = id;
        }

    }
}