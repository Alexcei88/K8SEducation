using System;
using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCreatedError
        : IBrokerMessage
    {
        public const string TYPE = "OrderCreatedError";

        public Guid UserId { get; set; }

        public string Message { get; set; }
        public override string MessageType => TYPE;
    }
}