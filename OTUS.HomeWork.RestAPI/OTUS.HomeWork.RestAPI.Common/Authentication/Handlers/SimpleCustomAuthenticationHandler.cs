using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;

namespace OTUS.HomeWork.RestAPI.Abstraction.Authentication.Handlers
{
    public class SimpleCustomAuthenticationHandler
        : AuthenticationHandler<RestAPIAuthOption>
    {
        public const string AuthentificationScheme = "RestAPIScheme";

        private readonly IUserService _userService;

        public SimpleCustomAuthenticationHandler(
            IOptionsMonitor<RestAPIAuthOption> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }
            
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();
 
            if (!Request.Headers.ContainsKey(Constants.X_AUTH_HEADER))
                return AuthenticateResult.Fail("Missing Authorization Header");

            User user;
            var token = new AuthToken();
            try
            {
                var authHeader = Request.Headers[Constants.X_AUTH_HEADER].ToString();
                token = token.Decode(authHeader);
                user = await _userService.GetUserAsync(token.UserId);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (user == null || token.ExpiredUTCDateTime < DateTime.UtcNow)
                return AuthenticateResult.Fail("Invalid Authorization Header");

            var claims = new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
