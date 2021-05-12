using System;

namespace OTUS.HomeWork.EShop.Domain.DTO
{
    public record PaymentResultDTO
    {
        public Guid UserId { get; init; }
        public Guid BillingId { get; init; }

        public DateTime PaymentDateUtc { get; init; }

        public bool IsSuccessfully { get; init; }

        public string ErrorDescription { get; init; }
    }
}
