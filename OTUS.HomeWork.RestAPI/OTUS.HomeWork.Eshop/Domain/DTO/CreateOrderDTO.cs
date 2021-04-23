namespace OTUS.HomeWork.EShop.Domain
{
    public record CreateOrderDTO
    {        
        public string IdempotencyKey { get; set; }
    }
}
