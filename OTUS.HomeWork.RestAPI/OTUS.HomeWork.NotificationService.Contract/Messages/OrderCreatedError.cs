using System;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCreatedError
        : BrokerMessage
    {
        public const string TYPE = "OrderCreatedError";

        public string UserEmail { get; set; }

        public string Message { get; set; }
        public override string MessageType => TYPE;

        public override string Id { get; set; }

        public OrderCreatedError()
        { }

        public OrderCreatedError(string id)
        {
            Id = id;
        }

    }
}