using System;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCreatedError
        : NotificationMessage
    {
        public const string TYPE = "OrderCreatedError";

        public Guid UserId { get; set; }

        public string Message { get; set; }
        public override string MessageType => TYPE;
    }
}