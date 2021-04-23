using System;

namespace OTUS.HomeWork.BillingService.Domain
{
    public record BillingUserDTO
    {
        public Guid UserId { get; init; }
        
        public decimal Balance { get; init; }
    }
}