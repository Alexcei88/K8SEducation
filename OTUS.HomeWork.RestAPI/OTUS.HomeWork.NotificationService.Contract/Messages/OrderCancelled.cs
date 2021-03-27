namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCancelled
        : NotificationMessage
    {
        public const string TYPE = "OrderCancelled";

        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public int BillingAddressId { get; set; }

        public override string MessageType => TYPE;

    }
}