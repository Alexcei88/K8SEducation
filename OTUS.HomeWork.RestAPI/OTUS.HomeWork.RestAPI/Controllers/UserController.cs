using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.RestAPI.Domain;
using System;
using System.Threading.Tasks;
using OTUS.HomeWork.UserService;
using OTUS.HomeWork.UserService.Domain;
using Microsoft.Extensions.Logging;

namespace OTUS.HomeWork.RestAPI.Controllers
{
    [Authorize(Policy = "OnlyOwner")]
    [Route("api/user")]
    [ApiController]
    public class UserController 
        : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, IMapper mapper, ILogger<UserController> logger)		
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }
       
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDTO>> Get(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user != null)
                return Ok(_mapper.Map<UserDTO>(user));
            else
                return NotFound();
        }

        [HttpPut("{userId}")]
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
        public async Task<ActionResult> Delete(Guid userId)
        {
            int count = await _userService.DeleteUserAsync(userId);
            if (count <= 0)
                return NotFound();

            return NoContent();
        }
    }
}
