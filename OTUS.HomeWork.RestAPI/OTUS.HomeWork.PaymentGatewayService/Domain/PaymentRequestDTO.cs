namespace OTUS.HomeWork.PaymentGatewayService.Domain
{
    public class PaymentRequestDTO
    {        
        public decimal Amount { get; set; }

        public string IdempotenceKey { get; set; }
    }
}