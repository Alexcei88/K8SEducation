using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.AuthService.Domain;
using OTUS.HomeWork.Clients;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.RestAPI.Abstraction;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;

namespace OTUS.HomeWork.AuthService.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly BillingServiceClient _billingServiceClient;

        public AuthController(IUserService userService, IMapper mapper
            , ILogger<AuthController> logger
            , BillingServiceClient billingServiceClient)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
            _billingServiceClient = billingServiceClient;
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<ActionResult<Domain.UserDTO>> Register([FromBody]RegisterUserDTO user)
        {
            var newUser = await _userService.CreateUserAsync(_mapper.Map<User>(user));
            try
            {
                var _ = await _billingServiceClient.CreateUserAsync(newUser.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Не удалось создать пользователя в BillingService");
                await _userService.DeleteUserAsync(newUser.Id);
                throw;
            }
            return Ok(_mapper.Map<Domain.UserDTO>(newUser));
        }

        [HttpGet]
        [Route("api/login")]
        public async Task<IActionResult> Authentification()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return Unauthorized();

            User user;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];
                user = await _userService.Authenticate(Guid.Parse(username), password);
                if (user == null)
                {
                    _logger.LogError("Specified user hasn't found");
                    return Unauthorized("Invalid Username or Password");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error parse authorization header");
                return Unauthorized("Invalid Authorization Header");
            }            
            Response.Headers.Add(Constants.X_AUTH_HEADER, new AuthToken
            {
                UserId = user.Id,
                ExpiredUTCDateTime = DateTime.UtcNow.AddMinutes(60)
            }.Encode());

            return Ok();
        }

    }
}
