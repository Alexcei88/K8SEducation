namespace OTUS.HomeWork.NotificationService.Contract.Messages
{
    public class OrderCreated
    {
        public string OrderNumber { get; set; }
        
        public decimal Price { get; set; }
        
        public int BillingAddressId { get; set; }
    }
}