using System;

namespace OTUS.HomeWork.BillingService.Domain
{
    public record UserDTO
    {
        public Guid UserId { get; init; }
        
        public decimal Balance { get; init; }
    }
}