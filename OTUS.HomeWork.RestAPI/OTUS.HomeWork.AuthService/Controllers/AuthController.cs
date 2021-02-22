using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody]RegisterUserDTO user)
        {
            var newUser = await _userService.CreateUserAsync(_mapper.Map<User>(user));
            return _mapper.Map<UserDTO>(newUser);
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login([FromBody]AuthentificateUserDTO model)
        {
            var user = await _userService.Authenticate(model.UserId, model.Password);

            if (user == null)
                return BadRequest (new { message = "User or password is incorrect" });

            // на этом месте должен быть JWT
            Response.Headers.Add(Constants.X_AUTH_HEADER, new AuthToken
            {
                UserId = user.Id, ExpiredUTCDateTime = DateTime.UtcNow.AddMinutes(60)
            }.Encode());
            return Ok();
        }
    }
}
