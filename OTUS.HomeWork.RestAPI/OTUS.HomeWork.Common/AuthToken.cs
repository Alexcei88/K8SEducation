using System;
using System.Text;
using System.Text.Json;

namespace OTUS.HomeWork.Common
{
    public class AuthToken
    {
        public Guid UserId { get; set; }
        
        public DateTime ExpiredUTCDateTime { get; set; }
    }

    public static class TokenHelper
    {
        public static string Encode(this AuthToken token)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(token)));
        }

        public static AuthToken Decode(this AuthToken token, string input)
        {
            token = JsonSerializer.Deserialize<AuthToken>(Convert.FromBase64String(input));
            return token;
        }
    }
}