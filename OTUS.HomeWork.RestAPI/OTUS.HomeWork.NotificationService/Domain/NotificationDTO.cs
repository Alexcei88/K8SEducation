using System;

namespace OTUS.HomeWork.NotificationService.Domain
{
    public record NotificationDTO
    {
        public string Message { get; init; }

        public DateTime CreatedDateUtc { get; init; }
    }
}