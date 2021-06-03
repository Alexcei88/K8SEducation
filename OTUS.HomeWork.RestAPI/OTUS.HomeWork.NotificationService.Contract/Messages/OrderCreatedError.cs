using System;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCreatedError
        : BrokerMessage
    {
        public const string TYPE = "OrderCreatedError";

        public Guid UserId { get; set; }

        public string Message { get; set; }
        public override string MessageType => TYPE;

        public override string Id { get; }

        public OrderCreatedError()
        { }

        public OrderCreatedError(string id)
        {
            Id = id;
        }

    }
}