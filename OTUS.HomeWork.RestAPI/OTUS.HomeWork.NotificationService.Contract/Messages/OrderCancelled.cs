using OTUS.HomeWork.Common;

namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCancelled
        : IBrokerMessage
    {
        public const string TYPE = "OrderCancelled";

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public int BillingAddressId { get; set; }

        public override string MessageType => TYPE;

    }
}