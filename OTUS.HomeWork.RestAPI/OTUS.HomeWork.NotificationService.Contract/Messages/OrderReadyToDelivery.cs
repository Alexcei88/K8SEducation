using OTUS.HomeWork.Common;
using System;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderReadyToDelivery
        : BrokerMessage
    {
        public const string TYPE = "OrderReadyToDelivery";

        public string OrderNumber { get; set; }

        public string UserEmail { get; set; }

        public override string MessageType => TYPE;

        public override string Id { get; set; }

        public OrderReadyToDelivery()
        { }


        public OrderReadyToDelivery(string id)
        {
            Id = id;
        }

    }
}
