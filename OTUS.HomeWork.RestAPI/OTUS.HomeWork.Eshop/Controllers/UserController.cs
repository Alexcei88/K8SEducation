using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTUS.HomeWork.EShop.Domain.DTO;
using OTUS.HomeWork.RestAPI.Abstraction;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private HttpContext _hcontext;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService
            , IMapper mapper
            , IHttpContextAccessor haccess
            , ILogger<UserController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
            _hcontext = haccess.HttpContext;
        }
       
        [HttpPut("signin")]
        public async Task<IActionResult> Authentification()
        {            
            var nameIdentifier = _hcontext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var newUser = await _userService.CreateUserAsync(new User
            {
                UserName = nameIdentifier,
            });
            return Ok(_mapper.Map<UserDTO>(newUser));
        }

        [HttpGet("{userId}")]
        [Authorize(Policy = "OnlyOwner")]
        public async Task<ActionResult<UserDTO>> Get(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user != null)
                return Ok(_mapper.Map<UserDTO>(user));
            else
                return NotFound();
        }

        [HttpPut("{userId}")]
        [Authorize(Policy = "OnlyOwner")]
        public async Task<ActionResult<UserDTO>> Put(Guid userId, [FromBody] UserDTO user)
        {
            if (userId != user.UserId)
            {
                return BadRequest();
            }
            var updatedUser = await _userService.UpdateUserAsync(userId, _mapper.Map<User>(user));
            if (updatedUser == null)
                return NotFound();
            return _mapper.Map<UserDTO>(updatedUser);
        }

        [HttpDelete("{userId}")]
        [Authorize(Policy = "OnlyOwner")]
        public async Task<ActionResult> Delete(Guid userId)
        {
            int count = await _userService.DeleteUserAsync(userId);
            if (count <= 0)
                return NotFound();

            return NoContent();
        }
    }
}