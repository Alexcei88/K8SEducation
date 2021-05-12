namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record CreateOrderDTO
    {        
        public string IdempotencyKey { get; set; }
        
        public string DeliveryAddress { get; set; }
    }
}
