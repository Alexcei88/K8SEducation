using System;

namespace OTUS.HomeWork.AuthService.Domain
{
    public class AuthentificateUserDTO
    {
        public Guid UserId { get; set; }
        public string Password { get; set; }
    }
}
