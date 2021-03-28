using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.NotificationService.Domain
{
    public record Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        public Guid UserId { get; init; }

        public string Message { get; init; }

        public DateTime CreatedDateUtc { get; init; }
    }
}