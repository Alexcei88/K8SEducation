using System;

namespace OTUS.HomeWork.BillingService.Domain
{
    public record UserDTO
    {
        public Guid Id { get; init; }
        
        public decimal Balance { get; init; }
    }
}