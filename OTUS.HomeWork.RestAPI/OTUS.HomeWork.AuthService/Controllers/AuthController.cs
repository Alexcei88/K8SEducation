using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.AuthService.Domain;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.RestAPI.Domain;
using OTUS.HomeWork.UserService;
using OTUS.HomeWork.UserService.Domain;

namespace OTUS.HomeWork.AuthService.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IMapper mapper, ILogger<AuthController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody]RegisterUserDTO user)
        {
            var newUser = await _userService.CreateUserAsync(_mapper.Map<User>(user));
            return Ok(_mapper.Map<UserDTO>(newUser));
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
