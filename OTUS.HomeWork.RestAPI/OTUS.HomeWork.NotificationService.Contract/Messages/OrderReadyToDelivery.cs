using OTUS.HomeWork.Common;
using System;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderReadyToDelivery
        : IBrokerMessage
    {
        public const string TYPE = "OrderReadyToDelivery";

        public string OrderNumber { get; set; }

        public Guid UserId { get; set; }

        public override string MessageType => TYPE;

    }
}
